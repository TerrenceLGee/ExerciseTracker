using System.Globalization;
using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Results;

namespace ExerciseTracker.Presentation.Helpers;

public class ExerciseTrackerAppHelper
{
    public static Result<DateTime> BuildDateTime(string dateString, string dateFormat)
    {
        CultureInfo info = CultureInfo.InvariantCulture;

        if (!ExerciseTrackerUIValidator.IsValidDateString(dateString, dateFormat, info))
            return Result<DateTime>.Fail($"Invalid date, date must match format: {dateFormat}");

        var validTime = DateTime.ParseExact(dateString, dateFormat, info);

        return Result<DateTime>.Ok(validTime);
    }

    public static Result<DateTime?> BuildUpdateDateTime(string dateString, string dateFormat)
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


}