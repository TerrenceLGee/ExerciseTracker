using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Models;
using ExerciseTracker.Domain.Services;
using ExerciseTracker.Presentation.Helpers;
using ExerciseTracker.Presentation.Options;
using ExerciseTracker.Presentation.Options.Extensions;
using Spectre.Console;

namespace ExerciseTracker.Presentation.UI;

public class ExerciseTrackerUI : IExerciseTrackerUI
{
    private readonly IExerciserService _exerciserService;
    private readonly IExerciseService _exerciseService;
    private const string DateFormat = "MM-dd-yyyy HH:mm";
    private const string BirthDateFormat = "MM-dd-yyyy";
    private const string DurationFormat = @"hh\:mm";
    private readonly IReadOnlyDictionary<MenuOption, Func<CancellationToken, Task>> _methods;

    public ExerciseTrackerUI(IExerciserService exerciserService, IExerciseService exerciseService)
    {
        _exerciserService = exerciserService;
        _exerciseService = exerciseService;

        _methods = new Dictionary<MenuOption, Func<CancellationToken, Task>>
        {
            [MenuOption.AddExerciser] = CreateExerciserAsync,
            [MenuOption.UpdateExerciser] = UpdateExerciserAsync,
            [MenuOption.DeleteExerciser] = DeleteExerciserAsync,
            [MenuOption.GetExerciserById] = ViewExerciserByIdAsync,
            [MenuOption.GetAllExercisers] = ViewAllExercisersAsync,
            [MenuOption.CreateExerciseTracker] = TrackExerciseSessionAsync,
            [MenuOption.UpdateExerciseTracker] = UpdateTrackedExerciseSessionAsync,
            [MenuOption.DeleteExerciseTracker] = DeleteTrackedExerciseSessionAsync,
            [MenuOption.GetExerciseTrackerById] = ViewTrackedExerciseSessionByIdAsync,
            [MenuOption.GetExerciseTrackersByExerciserId] = ViewTrackedExerciseSessionsByExerciserIdAsync,
            [MenuOption.GetAllExerciseTrackers] = ViewAllTrackedExerciseSessionsAsync,
            [MenuOption.Exit] = _ => Task.FromResult(0),
        };
    }


    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            MenuOption userChoice;

            try
            {
                userChoice = DisplayMenuAndGetUserChoice();
            }
            catch (OperationCanceledException)
            {
                ExerciseTrackerUIHelper.DisplayMessage("Operation canceled. Exiting.", "yellow");
                return;
            }

            if (!_methods.TryGetValue(userChoice, out var method))
            {
                ExerciseTrackerUIHelper.DisplayMessage($"No method available for being able to  {userChoice.GetDisplayName()}");
                continue;
            }

            try
            {
                await method(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                ExerciseTrackerUIHelper.DisplayMessage("Operation canceled. Exiting.", "yellow");
                continue;
            }

            if (userChoice == MenuOption.Exit)
            {
                ExerciseTrackerUIHelper.DisplayMessage("Goodbye", "green");
                break;
            }

            ExerciseTrackerUIHelper.Pause();
        }
    }

    public async Task CreateExerciserAsync(CancellationToken cancellationToken = default)
    {
        var name = ExerciseTrackerUIHelper.GetInput<string>("Enter user's name: ");

        var birthDateString = ExerciseTrackerUIHelper.GetInput<string>($"Enter birth date in format {BirthDateFormat}: ");

        var bodyWeight = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to enter your body weight? ")
            ? ExerciseTrackerUIHelper.GetInput<double?>("Enter your body weight in pounds: ")
            : null;

        var fitnessGoal = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to enter your fitness goal? ")
            ? ExerciseTrackerUIHelper.GetInput<string?>("Enter your fitness goal: ")
            : null;

        if (!ExerciseTrackerUIHelper.GetOptionalInput($"Confirm adding user: {name}"))
        {
            ExerciseTrackerUIHelper.DisplayMessage("User addition canceled", "yellow");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var birthDateResult = ExerciseTrackerAppHelper.BuildDateTime(birthDateString, BirthDateFormat);

        if (ExerciseTrackerAppHelper.IsFailure(birthDateResult, out var birthDate))
            return;

        var createdExerciser = ExerciseTrackerAppHelper.BuildCreateExerciserRequest(name, birthDate, bodyWeight, fitnessGoal);

        var createdExerciserResult = await _exerciserService.CreateExerciserAsync(createdExerciser, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(createdExerciserResult))
            return;

        ExerciseTrackerUIHelper.DisplayMessage($"Successfully added user: {name}", "green");
    }

    public async Task UpdateExerciserAsync(CancellationToken cancellationToken = default)
    {
        var allExercisersResult = await _exerciserService.GetExercisersAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExercisersResult, out var exercisers))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercisers, "users", DisplayExercisers))
            return;

        var exerciserId = ExerciseTrackerUIHelper.GetInput<int>("Enter the id of user to update: ");

        if (!ExerciseTrackerAppHelper.IsValidExerciserId(exercisers, exerciserId))
            return;

        var name = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to update user name? ")
            ? ExerciseTrackerUIHelper.GetInput<string?>("Enter updated name: ")
            : null;

        var birthDateString = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to update the birth date? ")
            ? ExerciseTrackerUIHelper.GetInput<string?>($"Enter updated birth date in format {BirthDateFormat}: ")
            : null;

        var bodyWeight = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to update the body weight? ")
            ? ExerciseTrackerUIHelper.GetInput<double?>("Enter updated body weight: ")
            : null;

        var fitnessGoal = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to update the fitness goal? ")
            ? ExerciseTrackerUIHelper.GetInput<string?>("Enter updated fitness goal: ")
            : null;

        if (!ExerciseTrackerUIHelper.GetOptionalInput($"Confirm updating user# {exerciserId}?"))
        {
            ExerciseTrackerUIHelper.DisplayMessage("User update cancelled", "yellow");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        DateTime? birthDate = null;

        if (birthDateString is not null)
        {
            var birthDateResult = ExerciseTrackerAppHelper.BuildUpdateDateTime(birthDateString, BirthDateFormat);

            if (ExerciseTrackerAppHelper.IsFailure(birthDateResult, out birthDate))
                return;
        }

        var updatedExerciser =
            ExerciseTrackerAppHelper.BuildUpdateExerciserRequest(exerciserId, name, birthDate, bodyWeight, fitnessGoal);

        var updatedUserResult = await _exerciserService.UpdateExerciserAsync(updatedExerciser, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(updatedUserResult))
            return;

        ExerciseTrackerUIHelper.DisplayMessage($"Successfully updated user# {exerciserId}", "green");
    }

    public async Task DeleteExerciserAsync(CancellationToken cancellationToken = default)
    {
        var allExercisersResult = await _exerciserService.GetExercisersAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExercisersResult, out var exercisers))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercisers, "users", DisplayExercisers))
            return;

        var exerciserId = ExerciseTrackerUIHelper.GetInput<int>("Enter id of user to be deleted: ");

        if (!ExerciseTrackerAppHelper.IsValidExerciserId(exercisers, exerciserId))
            return;

        if (!ExerciseTrackerUIHelper.GetOptionalInput($"Confirm deleted user# {exerciserId}? "))
        {
            ExerciseTrackerUIHelper.DisplayMessage("User deletion cancelled", "yellow");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var deletedExerciserResult = await _exerciserService.DeleteExerciserAsync(exerciserId, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(deletedExerciserResult))
            return;

        ExerciseTrackerUIHelper.DisplayMessage($"Successfully deleted user# {exerciserId}", "green");
    }

    public async Task ViewExerciserByIdAsync(CancellationToken cancellationToken = default)
    {
        var allExercisersResult = await _exerciserService.GetExercisersAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExercisersResult, out var exercisers))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercisers, "users", DisplayExercisers))
            return;

        var exerciserId = ExerciseTrackerUIHelper.GetInput<int>("Enter id of user to view detailed information for: ");

        cancellationToken.ThrowIfCancellationRequested();

        if (!ExerciseTrackerAppHelper.IsValidExerciserId(exercisers, exerciserId))
            return;

        var exerciserResult = await _exerciserService.GetExerciserByIdAsync(exerciserId, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(exerciserResult, out var exerciser))
            return;

        DisplayExerciser(exerciser);
    }

    public async Task ViewAllExercisersAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var allExerciserResult = await _exerciserService.GetExercisersAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExerciserResult, out var exercises))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercises, "users", DisplayExercisers))
            return;
    }

    public async Task TrackExerciseSessionAsync(CancellationToken cancellationToken = default)
    {
        var allExercisersResult = await _exerciserService.GetExercisersAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExercisersResult, out var exercisers))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercisers, "users", DisplayExercisers))
            return;

        var exerciserId =
            ExerciseTrackerUIHelper.GetInput<int>(
                "Enter the id of the user who you wish to track an exercise session for: ");

        if (!ExerciseTrackerAppHelper.IsValidExerciserId(exercisers, exerciserId))
            return;

        var startTimeDateString =
            ExerciseTrackerUIHelper.GetInput<string>($"Enter exercise session start time in format {DateFormat} (24 hour clock: 0-23): ");

        var startTimeResult = ExerciseTrackerAppHelper.BuildDateTime(startTimeDateString, DateFormat);

        if (ExerciseTrackerAppHelper.IsFailure(startTimeResult, out var startTime))
            return;

        var endTimeDateString =
            ExerciseTrackerUIHelper.GetInput<string>($"Enter exercise session end time in format {DateFormat} (24 hour clock: 0-23): ");

        var endTimeResult = ExerciseTrackerAppHelper.BuildDateTime(endTimeDateString, DateFormat);

        if (ExerciseTrackerAppHelper.IsFailure(endTimeResult, out var endTime))
            return;

        var exerciseType = GetExerciseType();

        var comments =
            ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to add any comments about this exercise session? ")
                ? ExerciseTrackerUIHelper.GetInput<string?>("Enter comments: ")
                : null;

        if (!ExerciseTrackerUIHelper.GetOptionalInput($"Confirm tracking exercise session for user# {exerciserId}? "))
        {
            ExerciseTrackerUIHelper.DisplayMessage($"Exercise session tracking cancelled for user# {exerciserId}", "yellow");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var createdExerciseSessionTracker =
            ExerciseTrackerAppHelper.BuildCreateExerciseRequest(exerciserId, startTime, endTime, exerciseType,
                comments);

        var createdExerciseSessionTrackerResult =
            await _exerciseService.CreateExerciseAsync(createdExerciseSessionTracker, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(createdExerciseSessionTrackerResult))
            return;

        ExerciseTrackerUIHelper.DisplayMessage($"Successfully tracked exercise session for user# {exerciserId}", "green");
    }

    public async Task UpdateTrackedExerciseSessionAsync(CancellationToken cancellationToken = default)
    {
        var allExerciseSessionResult = await _exerciseService.GetExercisesAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExerciseSessionResult, out var exercises))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercises, "tracked exercise sessions",
                DisplayExerciseSessions))
            return;

        var id = ExerciseTrackerUIHelper.GetInput<int>("Enter tracked exercise session id to update: ");

        if (!ExerciseTrackerAppHelper.IsValidTrackedExerciseId(exercises, id))
            return;

        var sessionResult = await _exerciseService.GetExerciseByIdAsync(id, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(sessionResult, out var session))
            return;

        ExerciseTrackerUIHelper.DisplayMessage(
            $"Updating tracked exercise session# {id} for user# {session.ExerciserId}\n");

        var startTimeDateString = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to update the session start time? ")
            ? ExerciseTrackerUIHelper.GetInput<string?>($"Enter updated start time in format {DateFormat} (24 hour clock: 0-23): ")
            : null;

        DateTime? startTime = null;

        if (startTimeDateString is not null)
        {
            var startTimeDateResult = ExerciseTrackerAppHelper.BuildUpdateDateTime(startTimeDateString, DateFormat);

            if (ExerciseTrackerAppHelper.IsFailure(startTimeDateResult, out startTime))
                return;
        }

        var endTimeDateString = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to update the session end time? ")
            ? ExerciseTrackerUIHelper.GetInput<string?>($"Enter updated end time in format {DateFormat} (24 hour clock: 0-23): ")
            : null;

        DateTime? endTime = null;

        if (endTimeDateString is not null)
        {
            var endTimeDateResult = ExerciseTrackerAppHelper.BuildUpdateDateTime(endTimeDateString, DateFormat);

            if (ExerciseTrackerAppHelper.IsFailure(endTimeDateResult, out endTime))
                return;
        }

        ExerciseType? exerciseType = ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to update the exercise type? ")
            ? GetExerciseType()
            : null;

        var comments =
            ExerciseTrackerUIHelper.GetOptionalInput("Do you wish to update the comments on the exercise session? ")
                ? ExerciseTrackerUIHelper.GetInput<string?>("Enter updated comments: ")
                : null;

        if (!ExerciseTrackerUIHelper.GetOptionalInput(
                $"Confirm updating tracked exercise session# {id} for user# {session.ExerciserId}? "))
        {
            ExerciseTrackerUIHelper.DisplayMessage($"Exercise tracking session# {id} update for user# {session.ExerciserId} cancelled", "yellow");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var updatedSession =
            ExerciseTrackerAppHelper.BuildUpdateExerciseRequest(id, startTime, endTime, exerciseType, comments);

        var updatedSessionResult = await _exerciseService.UpdateExerciseAsync(updatedSession, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(updatedSessionResult))
            return;

        ExerciseTrackerUIHelper.DisplayMessage($"Successfully updated tracked exercise session# {id} for user# {session.Id}", "green");
    }

    public async Task DeleteTrackedExerciseSessionAsync(CancellationToken cancellationToken = default)
    {
        var allExercisesResult = await _exerciseService.GetExercisesAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExercisesResult, out var exercises))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercises, "tracked exercise sessions",
                DisplayExerciseSessions))
            return;

        var id = ExerciseTrackerUIHelper.GetInput<int>("Enter the id of the exercise session to delete: ");

        if (!ExerciseTrackerAppHelper.IsValidTrackedExerciseId(exercises, id))
            return;

        var sessionResult = await _exerciseService.GetExerciseByIdAsync(id, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(sessionResult, out var session))
            return;

        if (!ExerciseTrackerUIHelper.GetOptionalInput(
                $"Confirm deletion of session# {id} for user# {session.ExerciserId}? "))
        {
            ExerciseTrackerUIHelper.DisplayMessage($"Deletion of tracked exercise session# {id} for user# {session.ExerciserId} cancelled", "yellow");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var deletionResult = await _exerciseService.DeleteExerciseAsync(id, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(deletionResult))
            return;

        ExerciseTrackerUIHelper.DisplayMessage($"Successfully deleted session# {id} for user# {session.ExerciserId}",
            "green");
    }

    public async Task ViewTrackedExerciseSessionByIdAsync(CancellationToken cancellationToken = default)
    {
        var allExercisesRequest = await _exerciseService.GetExercisesAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExercisesRequest, out var exercises))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercises, "tracked exercise sessions",
                DisplayExerciseSessions))
            return;

        var id = ExerciseTrackerUIHelper.GetInput<int>(
            "Please choose the exercise session id to see detailed information: ");

        cancellationToken.ThrowIfCancellationRequested();

        if (!ExerciseTrackerAppHelper.IsValidTrackedExerciseId(exercises, id))
            return;

        var sessionResult = await _exerciseService.GetExerciseByIdAsync(id, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(sessionResult, out var session))
            return;

        DisplayExerciseSession(session);
    }

    public async Task ViewTrackedExerciseSessionsByExerciserIdAsync(CancellationToken cancellationToken = default)
    {
        var allExercisersResult = await _exerciserService.GetExercisersAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allExercisersResult, out var exercisers))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercisers, "users", DisplayExercisers))
            return;

        var exerciserId =
            ExerciseTrackerUIHelper.GetInput<int>(
                "Please enter the id of the user whom you wish to view tracked exercise sessions for: ");

        cancellationToken.ThrowIfCancellationRequested();

        if (!ExerciseTrackerAppHelper.IsValidExerciserId(exercisers, exerciserId))
            return;

        var exercisesResult = await _exerciseService.GetExercisesByExerciserIdAsync(exerciserId, cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(exercisesResult, out var exercises))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(exercises, $"tracked exercise sessions for user# {exerciserId}",
                DisplayExerciseSessions))
            return;
    }

    public async Task ViewAllTrackedExerciseSessionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var allSessionsResult = await _exerciseService.GetExercisesAsync(cancellationToken);

        if (ExerciseTrackerAppHelper.IsFailure(allSessionsResult, out var sessions))
            return;

        if (!ExerciseTrackerUIHelper.ShowPaginatedItems(sessions, "tracked exercise sessions", DisplayExerciseSessions))
            return;
    }

    private ExerciseType GetExerciseType()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<ExerciseType>()
                .Title("Please choose exercise type: ")
                .AddChoices(Enum.GetValues<ExerciseType>()));
    }

    private MenuOption DisplayMenuAndGetUserChoice()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<MenuOption>()
                .Title("Please choose one of the following options:")
                .AddChoices(Enum.GetValues<MenuOption>())
                .UseConverter(choice => choice.GetDisplayName()));
    }

    private void DisplayExerciser(ExerciserDto exerciser)
    {
        var bodyweightDisplay = exerciser.BodyWeight is null
            ? "Not available"
            : $"{exerciser.BodyWeight} pounds";
        var fitnessGoalDisplay = exerciser.FitnessGoal is null
            ? "Not available"
            : $"{exerciser.FitnessGoal}";

        ExerciseTrackerUIHelper.DisplayMessage($"Information for user# {exerciser.Id}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Name: {exerciser.Name}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Age: {exerciser.Age}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Body weight: {bodyweightDisplay}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Fitness goal: {fitnessGoalDisplay}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Total number of tracked exercises sessions: {exerciser.NumberOfSessions}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Total duration of all tracked exercise sessions: {exerciser.TotalExerciseDuration.ToString(DurationFormat)}", "blue");

        ExerciseTrackerUIHelper.DisplayMessage($"\nExercise sessions: ", "yellow");
        if (exerciser.Exercises.Count == 0)
        {
            ExerciseTrackerUIHelper.DisplayMessage($"No exercise sessions tracked so far", "yellow");
        }
        else
        {
            var table = new Table().Expand();
            table.AddColumn("[springgreen3]Start Time[/]");
            table.AddColumn("[springgreen3]End time[/]");
            table.AddColumn("[springgreen3]Duration[/]");
            table.AddColumn("[springgreen3]Exercise type[/]");
            table.AddColumn("[springgreen3]Comments[/]");

            foreach (var session in exerciser.Exercises)
            {
                table.AddRow(
                    $"[springgreen3]{session.StartTime.ToString(DateFormat)}[/]",
                    $"[springgreen3]{session.EndTime.ToString(DateFormat)}[/]",
                    $"[springgreen3]{session.Duration.ToString(DurationFormat)}[/]",
                    $"[springgreen3]{session.ExerciseType.ToString()}[/]",
                    $"[springgreen3]{session.Comments ?? "Not available"}[/]");
            }

            AnsiConsole.Write(table);
        }
    }

    private void DisplayExerciseSession(ExerciseDto session)
    {
        ExerciseTrackerUIHelper.DisplayMessage($"Information for exercise session# {session.Id}, for user# {session.ExerciserId}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"User name: {session.ExerciserName}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"User age: {session.ExerciserAge}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Exercise type: {session.ExerciseType.ToString()}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"User comments: {session.Comments}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Session start time: {session.StartTime.ToString(DateFormat)}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Session end time: {session.EndTime.ToString(DateFormat)}", "blue");
        ExerciseTrackerUIHelper.DisplayMessage($"Session duration: {session.Duration.ToString(DurationFormat)}", "blue");
    }

    private void DisplayExercisers(IReadOnlyList<ExerciserDto> exercisers)
    {
        var table = new Table().Expand();
        table.AddColumn("[aquamarine1]Id[/]");
        table.AddColumn("[aquamarine1]Name[/]");
        table.AddColumn("[aquamarine1]Age[/]");
        table.AddColumn("[aquamarine1]Body Weight[/]");
        table.AddColumn("[aquamarine1]Fitness Goal[/]");
        table.AddColumn("[aquamarine1]Total Number Of Tracked Exercise Sessions[/]");
        table.AddColumn("[aquamarine1]Total Exercise Duration[/]");
        

        ExerciseTrackerUIHelper.DisplayMessage("All users:", "aquamarine1");

        foreach (var exerciser in exercisers)
        {
            var bodyweightDisplay = exerciser.BodyWeight is null
                ? "Not available"
                : $"{exerciser.BodyWeight} pounds";
            var fitnessGoalDisplay = exerciser.FitnessGoal is null
                ? "Not available"
                : $"{exerciser.FitnessGoal}";

            table.AddRow(
                $"[aquamarine1]{exerciser.Id}[/]",
                $"[aquamarine1]{exerciser.Name}[/]",
                $"[aquamarine1]{exerciser.Age.ToString()}[/]",
                $"[aquamarine1]{bodyweightDisplay}[/]",
                $"[aquamarine1]{fitnessGoalDisplay}[/]",
                $"[aquamarine1]{exerciser.NumberOfSessions}[/]",
                $"[aquamarine1]{exerciser.TotalExerciseDuration.ToString(DurationFormat)}[/]");
        }

        AnsiConsole.Write(table);
    }

    private void DisplayExerciseSessions(IReadOnlyList<ExerciseDto> sessions)
    {
        var table = new Table().Expand();
        table.AddColumn("[aquamarine1]Id[/]");
        table.AddColumn("[aquamarine1]User Id[/]");
        table.AddColumn("[aquamarine1]User Name[/]");
        table.AddColumn("[aquamarine1]User Age[/]");
        table.AddColumn("[aquamarine1]Exercise Type[/]");
        table.AddColumn("[aquamarine1]User comments[/]");
        table.AddColumn("[aquamarine1]Session Start Time[/]");
        table.AddColumn("[aquamarine1]Session End Time[/]");
        table.AddColumn("[aquamarine1]Session Duration[/]");

        ExerciseTrackerUIHelper.DisplayMessage("All exercise sessions:", "aquamarine1");

        foreach (var session in sessions)
        {
            table.AddRow(
                $"[aquamarine1]{session.Id.ToString()}[/]",
                $"[aquamarine1]{session.ExerciserId.ToString()}[/]",
                $"[aquamarine1]{session.ExerciserName}[/]",
                $"[aquamarine1]{session.ExerciserAge}[/]",
                $"[aquamarine1]{session.ExerciseType.ToString()}[/]",
                $"[aquamarine1]{session.Comments ?? "Not available"}[/]",
                $"[aquamarine1]{session.StartTime.ToString(DateFormat)}[/]",
                $"[aquamarine1]{session.EndTime.ToString(DateFormat)}[/]",
                $"[aquamarine1]{session.Duration.ToString(DurationFormat)}[/]");
        }

        AnsiConsole.Write(table);
    }
}