using System.Globalization;
using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Models;
using ExerciseTracker.Core.Results;

namespace ExerciseTracker.Presentation.Helpers;

public static class ExerciseTrackerAppHelper
{
    public static CreateExerciserRequest BuildCreateExerciserRequest(string name, DateTime birthDate, double? bodyWeight,
        string? fitnessGoal) => new()
    {
        Name = name,
        BirthDate = birthDate,
        BodyWeight = bodyWeight,
        FitnessGoal = fitnessGoal,
    };

    public static CreateExerciseRequest BuildCreateExerciseRequest(int exerciserId, DateTime startTime, DateTime endTime,
        ExerciseType exerciseType, string? comments) => new()
    {
        ExerciserId = exerciserId,
        StartTime = startTime,
        EndTime = endTime,
        ExerciseType = exerciseType,
        Comments = comments,
    };


    public static UpdateExerciserRequest BuildUpdateExerciserRequest(int id, string? name, DateTime? birthDate,
        double? bodyWeight, string? fitnessGoal) => new()
    {
        Id = id,
        Name = name,
        BirthDate = birthDate,
        BodyWeight = bodyWeight,
        FitnessGoal = fitnessGoal,
    };

    public static UpdateExerciseRequest BuildUpdateExerciseRequest(int id, DateTime? startTime, DateTime? endTime,
        ExerciseType? exerciseType, string? comments) => new()
    {
        Id = id,
        StartTime = startTime,
        EndTime = endTime,
        ExerciseType = exerciseType,
        Comments = comments,
    };

    public static Result<DateTime> BuildDateTime(string dateString, string dateFormat)
    {
        CultureInfo info = CultureInfo.InvariantCulture;

        if (!ExerciseTrackerUIValidator.IsValidDateString(dateString, dateFormat, info))
            return Result<DateTime>.Fail($"Invalid date, date must match format: {dateFormat}");

        var validTime = DateTime.ParseExact(dateString, dateFormat, info);

        return Result<DateTime>.Ok(validTime);
    }

    public static Result<DateTime?> BuildUpdateDateTime(string? dateString, string dateFormat)
    {
        CultureInfo info = CultureInfo.InvariantCulture;

        if (!ExerciseTrackerUIValidator.IsValidDateString(dateString, dateFormat, info))
            return Result<DateTime?>.Fail($"Invalid date, date must match format: {dateFormat}");

        var validTime = DateTime.ParseExact(dateString, dateFormat, info);

        return Result<DateTime?>.Ok(validTime);
    }

    public static bool IsFailure<T>(Result<T> result, out T value)
    {
        if (!result.IsSuccess)
        {
            ExerciseTrackerUIHelper.DisplayMessage(result.ErrorMessage, "red");
            value = default!;
            return true;
        }

        value = result.Value!;
        return false;
    }

    public static bool IsFailure(Result result)
    {
        if (!result.IsSuccess)
        {
            ExerciseTrackerUIHelper.DisplayMessage(result.ErrorMessage);
            return true;
        }

        return false;
    }

    public static bool IsListEmpty<T>(IReadOnlyList<T> items, string itemType)
    {
        if (items.Count == 0)
        {
            ExerciseTrackerUIHelper.DisplayMessage($"No {itemType} found", "yellow");
            return true;
        }

        return false;
    }

    public static bool IsValidExerciserId(IReadOnlyList<ExerciserDto> exercisers, int exerciserId)
    {
        if (IsListEmpty(exercisers, "users"))
            return false;

        if (!ExerciseTrackerUIValidator.IsValidNumericInput(exerciserId))
        {
            ExerciseTrackerUIHelper.DisplayMessage("User id must be greater than 0", "red");
            return false;
        }

        if (!exercisers.Any(e => e.Id == exerciserId))
        {
            ExerciseTrackerUIHelper.DisplayMessage($"There is no user with id: {exerciserId} available", "red");
            return false;
        }

        return true;
    }

    public static bool IsValidTrackedExerciseId(IReadOnlyList<ExerciseDto> exercises, int exerciseId)
    {
        if (IsListEmpty(exercises, "exercise sessions"))
            return false;

        if (!ExerciseTrackerUIValidator.IsValidNumericInput(exerciseId))
        {
            ExerciseTrackerUIHelper.DisplayMessage("Tracked exercise session id must be greater than 0");
            return false;
        }

        if (!exercises.Any(e => e.Id == exerciseId))
        {
            ExerciseTrackerUIHelper.DisplayMessage($"There is no tracked exercise session with id: {exerciseId}", "red");
            return false;
        }

        return true;
    }
}