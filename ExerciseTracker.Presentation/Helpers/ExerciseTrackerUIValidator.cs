﻿using System.Globalization;

namespace ExerciseTracker.Presentation.Helpers;

public static class ExerciseTrackerUIValidator
{
    public static bool IsValidInputString(string input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }

    public static bool IsValidNumericInput(int value)
    {
        return value > 0;
    }

    public static bool IsValidDateString(string dateString, string dateFormat, CultureInfo info)
    {
        return DateTime.TryParseExact(dateString, dateFormat, info, DateTimeStyles.None, out _);
    }

    public static bool IsValidEndTime(DateTime startTime, DateTime endTime)
    {
        return endTime > startTime;
    }
}