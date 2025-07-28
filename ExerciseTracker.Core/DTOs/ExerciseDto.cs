namespace ExerciseTracker.Core.DTOs;

public class ExerciseDto
{
    public int Id { get; set; }
    public int ExerciserId { get; set; }
    public string ExerciserName { get; set; } = string.Empty;
    public int ExerciserAge { get; set; }
    public string? ExerciseType { get; set; }
    public string? Comments { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
}