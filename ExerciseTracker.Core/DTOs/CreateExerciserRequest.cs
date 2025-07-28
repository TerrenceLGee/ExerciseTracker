using System.ComponentModel.DataAnnotations;

namespace ExerciseTracker.Core.DTOs;

public class CreateExerciserRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public DateTime BirthDate { get; set; }
    public double? BodyWeight { get; set; }
    public string? FitnessGoal { get; set; }
}