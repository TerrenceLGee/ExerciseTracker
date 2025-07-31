using ExerciseTracker.DataAccess.Data;
using ExerciseTracker.DataAccess.Repositories;
using ExerciseTracker.Domain.Services;
using ExerciseTracker.Presentation.UI;
using ExerciseTracker.Domain.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using AutoMapper;
using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Models;
using ExerciseTracker.Domain.Validation;
using FluentValidation;
using Serilog.Parsing;

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
        .AddTransient<IExerciserRepository, ExerciserRepository>()
        .AddTransient<IExerciseRepository, ExerciseRepository>()
        .AddTransient<IExerciserService, ExerciserService>()
        .AddTransient<IExerciseService, ExerciseService>()
        .AddTransient<IExerciseTrackerUI, ExerciseTrackerUI>()
        .AddValidatorsFromAssemblyContaining<CreateExerciserRequestValidator>()
        .AddValidatorsFromAssemblyContaining<CreateExerciseRequestValidator>()
        .AddValidatorsFromAssemblyContaining<UpdateExerciserRequestValidator>()
        .AddValidatorsFromAssemblyContaining<UpdateExerciseRequestValidator>()
        .AddAutoMapper(cfg => { }, typeof(CoreMappingProfile));

    var serviceProvider = services.BuildServiceProvider();

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




