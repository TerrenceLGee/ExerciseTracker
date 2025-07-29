using System.ComponentModel.DataAnnotations;

namespace ExerciseTracker.Core.DTOs;

public class UpdateExerciserRequest
{
    [Required]
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime? BirthDate { get; set; }
    public double? BodyWeight { get; set; }
    public string? FitnessGoal { get; set; }
}