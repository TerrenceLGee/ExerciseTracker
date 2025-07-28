namespace ExerciseTracker.Domain.Helpers;

public static class ExerciseTrackerHelper
{
    public static int CalculateValidAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}