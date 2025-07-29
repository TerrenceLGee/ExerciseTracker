using System.ComponentModel.DataAnnotations;

namespace ExerciseTracker.Presentation.Options;

public enum MenuOption
{
    [Display(Name = "Add an user to the system")]
    AddUser,
    [Display(Name = "Update a user")]
    UpdateUser,
    [Display(Name = "Delete a user")]
    DeleteUser,
    [Display(Name = "View detailed information for user based on id")]
    GetUserById,
    [Display(Name = "View all users in the system")]
    GetAllUsers,
    [Display(Name = "Track an exercise session")]
    CreateExerciseTracker,
    [Display(Name = "Update an exercise session")]
    UpdateExerciseTracker,
    [Display(Name = "Delete an exercise session")]
    DeleteExerciseTracker,
    [Display(Name = "View detailed information for an exercise session based on id")]
    GetExerciseTrackerById,
    [Display(Name = "View all exercise sessions of a particular user")]
    GetExerciseTrackersByUserId,
    [Display(Name = "View all exercises sessions in the system")]
    GetAllExerciseTrackers,
    [Display(Name = "Exit the program")]
    Exit,
}