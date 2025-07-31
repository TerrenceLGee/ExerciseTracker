using AutoMapper;
using ExerciseTracker.DataAccess.Data;
using ExerciseTracker.DataAccess.Repositories;
using ExerciseTracker.Domain.MappingProfiles;
using ExerciseTracker.Domain.Services;
using ExerciseTracker.Domain.Validation;
using ExerciseTracker.Presentation.UI;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

LoggingSetup();

try
{
    await Startup();
}
catch (Exception ex)
{
    Log.Fatal($"Application terminated unexpectedly during startup: {ex.Message}");
    return;
}
finally
{
    Log.CloseAndFlush();
}



async Task Startup()
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var services = new ServiceCollection()
        .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
        .AddDbContext<ExerciseTrackerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")))
        .AddScoped<IExerciserRepository, ExerciserRepository>()
        .AddScoped<IExerciseRepository, ExerciseRepository>()
        .AddScoped<IExerciserService, ExerciserService>()
        .AddScoped<IExerciseService, ExerciseService>()
        .AddScoped<IExerciseTrackerUI, ExerciseTrackerUI>()
        .AddValidatorsFromAssemblyContaining<CreateExerciserRequestValidator>()
        .AddValidatorsFromAssemblyContaining<CreateExerciseRequestValidator>()
        .AddValidatorsFromAssemblyContaining<UpdateExerciserRequestValidator>()
        .AddValidatorsFromAssemblyContaining<UpdateExerciseRequestValidator>()
        .AddAutoMapper(cfg => { }, typeof(CoreMappingProfile).Assembly);

    var serviceProvider = services.BuildServiceProvider();

    serviceProvider.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();

    using var tokenSource = new CancellationTokenSource();

    Console.CancelKeyPress += (sender, e) =>
    {
        e.Cancel = true;
        tokenSource.Cancel();
    };

    var exerciseTrackerUI = serviceProvider.GetService<IExerciseTrackerUI>();

    if (exerciseTrackerUI is not null)
    {
        await exerciseTrackerUI.RunAsync(tokenSource.Token);
    }
    else
    {
        throw new Exception("An unexpected error occurred when attempting to run the program");
    }
}
void LoggingSetup()
{
    var loggingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    Directory.CreateDirectory(loggingDirectory);
    var filePath = Path.Combine(loggingDirectory, "exercise_tracker-.txt");
    var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File(
            path: filePath,
            rollingInterval: RollingInterval.Day,
            outputTemplate: outputTemplate)
        .CreateLogger();

}
