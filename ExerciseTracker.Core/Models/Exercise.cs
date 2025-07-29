using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExerciseTracker.Core.Models;

public class Exercise
{
    public int Id { get; set; }
    [Required]
    public int ExerciserId { get; set; }

    public Exerciser Exerciser { get; set; } = null!;
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }

    [Required] public ExerciseType ExerciseType { get; set; }
    public string? Comments { get; set; }
    [NotMapped] public TimeSpan Duration => EndTime - StartTime;
}

public enum ExerciseType
{
    Weights,
    Cardio,
    Yoga,
    Calisthenics,
    Other,
}