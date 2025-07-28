using System.ComponentModel.DataAnnotations;

namespace ExerciseTracker.Core.Models;

public class Exerciser
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public DateTime BirthDate { get; set; }
    public double? BodyWeight { get; set; }
    public string? FitnessGoal { get; set; }
    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}