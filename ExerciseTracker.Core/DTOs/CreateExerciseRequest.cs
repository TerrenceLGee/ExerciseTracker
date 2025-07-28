using System.ComponentModel.DataAnnotations;

namespace ExerciseTracker.Core.DTOs;

public class CreateExerciseRequest
{
    [Required]
    public int ExerciserId { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    public string? ExerciseType { get; set; }
    public string? Comments { get; set; }
}