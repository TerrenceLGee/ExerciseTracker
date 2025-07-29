using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExerciseTracker.DataAccess.Data;

public class ExerciseTrackerDbContextFactory : IDesignTimeDbContextFactory<ExerciseTrackerDbContext>
{
    public ExerciseTrackerDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ExerciseTracker.Presentation");

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ExerciseTrackerDbContext>();
        var connectionString = config.GetConnectionString("DatabaseConnection");
        optionsBuilder.UseSqlServer(connectionString);

        return new ExerciseTrackerDbContext(optionsBuilder.Options);
    }
}