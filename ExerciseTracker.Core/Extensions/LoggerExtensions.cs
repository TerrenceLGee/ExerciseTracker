using ExerciseTracker.Core.Results;
using Microsoft.Extensions.Logging;

namespace ExerciseTracker.Core.Extensions;

public static class LoggerExtensions
{
    public static Result LogErrorAndReturnFail(this ILogger logger, string errorMessage)
    {
        logger.LogError(errorMessage);
        return Result.Fail(errorMessage);
    }

    public static Result LogErrorAndReturnFail(this ILogger logger, string errorMessage, Exception ex)
    {
        logger.LogError(ex, errorMessage);
        return Result.Fail(errorMessage);
    }

    public static Result<T> LogErrorAndReturnFail<T>(this ILogger logger, string errorMessage)
    {
        logger.LogError(errorMessage);
        return Result<T>.Fail(errorMessage);
    }

    public static Result<T> LogErrorAndReturnFail<T>(this ILogger logger, string errorMessage, Exception ex)
    {
        logger.LogError(ex, errorMessage);
        return Result<T>.Fail(errorMessage);
    }
}