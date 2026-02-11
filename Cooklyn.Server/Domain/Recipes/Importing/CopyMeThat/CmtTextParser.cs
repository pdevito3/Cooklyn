namespace Cooklyn.Server.Domain.Recipes.Importing.CopyMeThat;

using System.Text.RegularExpressions;

public static partial class CmtTextParser
{
    public static CmtParsedRecipe Parse(string content)
    {
        var lines = content.Split('\n').Select(l => l.TrimEnd('\r')).ToList();

        if (lines.Count == 0)
            return new CmtParsedRecipe { Title = "Untitled" };

        var title = lines[0].Trim();
        string? source = null;
        string? servings = null;
        int? rating = null;
        var tags = new List<string>();
        var ingredientLines = new List<string>();
        string? steps = null;
        string? notes = null;

        // Parse sections after the title
        var section = Section.Header;
        var stepsLines = new List<string>();
        var notesLines = new List<string>();

        for (var i = 1; i < lines.Count; i++)
        {
            var line = lines[i];
            var trimmed = line.Trim();

            // Detect section transitions
            if (string.Equals(trimmed, "INGREDIENTS", StringComparison.OrdinalIgnoreCase))
            {
                section = Section.Ingredients;
                continue;
            }

            if (string.Equals(trimmed, "STEPS", StringComparison.OrdinalIgnoreCase))
            {
                section = Section.Steps;
                continue;
            }

            if (string.Equals(trimmed, "NOTES", StringComparison.OrdinalIgnoreCase))
            {
                section = Section.Notes;
                continue;
            }

            switch (section)
            {
                case Section.Header:
                    ParseHeaderLine(trimmed, ref source, ref servings, ref rating, tags);
                    break;

                case Section.Ingredients:
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        ingredientLines.Add(trimmed);
                    break;

                case Section.Steps:
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        stepsLines.Add(StripStepNumber(trimmed));
                    break;

                case Section.Notes:
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        notesLines.Add(trimmed);
                    break;
            }
        }

        if (stepsLines.Count > 0)
            steps = string.Join("\n", stepsLines);

        if (notesLines.Count > 0)
            notes = string.Join("\n", notesLines);

        return new CmtParsedRecipe
        {
            Title = title,
            Source = source,
            Servings = servings,
            Rating = rating,
            Tags = tags,
            IngredientLines = ingredientLines,
            Steps = steps,
            Notes = notes
        };
    }

    private static void ParseHeaderLine(string trimmed, ref string? source, ref string? servings,
        ref int? rating, List<string> tags)
    {
        if (string.IsNullOrWhiteSpace(trimmed))
            return;

        // "tags: Tag1, Tag2"
        if (trimmed.StartsWith("tags:", StringComparison.OrdinalIgnoreCase))
        {
            var tagText = trimmed["tags:".Length..].Trim();
            if (!string.IsNullOrWhiteSpace(tagText))
            {
                tags.AddRange(tagText.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            }
            return;
        }

        // Rating: "Rated 5/5" or "I made this. Rated 4/5" etc.
        var ratingMatch = RatingPattern().Match(trimmed);
        if (ratingMatch.Success)
        {
            if (int.TryParse(ratingMatch.Groups[1].Value, out var r))
                rating = r;
            return;
        }

        // "I made this." (without rating)
        if (trimmed.StartsWith("I made this", StringComparison.OrdinalIgnoreCase))
            return;

        // "Servings: X"
        if (trimmed.StartsWith("Servings:", StringComparison.OrdinalIgnoreCase))
        {
            servings = trimmed["Servings:".Length..].Trim();
            return;
        }

        // "Adapted from URL" or a raw URL as source
        if (trimmed.StartsWith("Adapted from ", StringComparison.OrdinalIgnoreCase))
        {
            source = trimmed["Adapted from ".Length..].Trim();
            return;
        }

        // If it looks like a URL, treat as source
        if (source == null && (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                              trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
        {
            source = trimmed;
        }
    }

    private static string StripStepNumber(string step)
    {
        // Remove leading "1) ", "2) ", etc.
        var match = StepNumberPattern().Match(step);
        return match.Success ? step[match.Length..] : step;
    }

    [GeneratedRegex(@"Rated\s+(\d)/5", RegexOptions.IgnoreCase)]
    private static partial Regex RatingPattern();

    [GeneratedRegex(@"^\d+\)\s*")]
    private static partial Regex StepNumberPattern();

    private enum Section
    {
        Header,
        Ingredients,
        Steps,
        Notes
    }
}
