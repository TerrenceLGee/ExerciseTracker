using Spectre.Console;

namespace ExerciseTracker.Presentation.Helpers;

public static class ExerciseTrackerUIHelper
{
    public static T GetInput<T>(string message, string color = "teal")
    {
        return AnsiConsole.Ask<T>(message);
    }

    public static void DisplayMessage(string? message, string color = "teal")
    {
        AnsiConsole.MarkupLine($"[{color}]{message}[/]");
    }

    public static bool GetOptionalInput(string optional, string color = "teal")
    {
        return AnsiConsole.Confirm($"[{color}]{optional}[/]");
    }

    public static bool ShowPaginatedItems<T>(IReadOnlyList<T> items, string entityName,
        Action<IReadOnlyList<T>> display, int pageSize = 10)
    {
        if (ExerciseTrackerAppHelper.IsListEmpty(items, entityName))
            return false;

        int pageIndex = 0;
        int pageCount = (int)Math.Ceiling(items.Count / (double)pageSize);

        while (true)
        {
            var pageItems = items
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            DisplayMessage($"Page {pageIndex + 1} of {pageCount} (showing {pageItems.Count} of {items.Count})", "blue");

            display(pageItems);

            var prompt = new SelectionPrompt<string>()
                .Title("Navigate pages: ");

            if (pageIndex > 0)
                prompt.AddChoice("Previous");

            prompt.AddChoice("Exit");

            if (pageIndex < pageCount - 1)
                prompt.AddChoice("Next");

            var choice = AnsiConsole.Prompt(prompt);

            if (choice == "Next" && pageIndex < pageCount - 1)
                pageIndex++;
            else if (choice == "Previous" && pageIndex > 0)
                pageIndex--;
            else break;
        }

        return true;
    }
}