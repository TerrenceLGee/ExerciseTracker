using System.ComponentModel.DataAnnotations.Schema;

namespace ExerciseTracker.Core.DTOs;

public class UpdateExerciseRequest
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? ExerciseType { get; set; }
    public string? Comments { get; set; }
}