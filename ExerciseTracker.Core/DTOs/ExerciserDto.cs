namespace ExerciseTracker.Core.DTOs;

public class ExerciserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public double? BodyWeight { get; set; }
    public string? FitnessGoal { get; set; }
    public TimeSpan TotalExerciseDuration { get; set; }
    public int NumberOfSessions { get; set; }

    public ICollection<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();
}