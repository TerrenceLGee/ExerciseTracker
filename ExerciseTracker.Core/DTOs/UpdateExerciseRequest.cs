using System.ComponentModel.DataAnnotations;
using ExerciseTracker.Core.Models;

namespace ExerciseTracker.Core.DTOs;

public class UpdateExerciseRequest
{
    [Required]
    public int Id { get; set; }
    [Required]
    public int ExerciserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ExerciseType? ExerciseType { get; set; }
    public string? Comments { get; set; }
}