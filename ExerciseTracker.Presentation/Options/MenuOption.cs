using System.ComponentModel.DataAnnotations;

namespace ExerciseTracker.Presentation.Options;

public enum MenuOption
{
    [Display(Name = "Add an user to the system")]
    AddExerciser,
    [Display(Name = "Update a user")]
    UpdateExerciser,
    [Display(Name = "Delete a user")]
    DeleteExerciser,
    [Display(Name = "View detailed information for user based on id")]
    GetExerciserById,
    [Display(Name = "View all users in the system")]
    GetAllExercisers,
    [Display(Name = "Track an exercise session")]
    CreateExerciseTracker,
    [Display(Name = "Update an exercise session")]
    UpdateExerciseTracker,
    [Display(Name = "Delete an exercise session")]
    DeleteExerciseTracker,
    [Display(Name = "View detailed information for an exercise session based on id")]
    GetExerciseTrackerById,
    [Display(Name = "View all exercise sessions of a particular user")]
    GetExerciseTrackersByExerciserId,
    [Display(Name = "View all exercises sessions in the system")]
    GetAllExerciseTrackers,
    [Display(Name = "Exit the program")]
    Exit,
}