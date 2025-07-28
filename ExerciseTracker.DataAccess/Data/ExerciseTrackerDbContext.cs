using Microsoft.EntityFrameworkCore;
using ExerciseTracker.Core.Models;

namespace ExerciseTracker.DataAccess.Data;

public class ExerciseTrackerDbContext : DbContext
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Exerciser> Exercisers { get; set; }

    public ExerciseTrackerDbContext(DbContextOptions<ExerciseTrackerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.Exerciser)
            .WithMany(ex => ex.Exercises)
            .HasForeignKey(e => e.ExerciserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}