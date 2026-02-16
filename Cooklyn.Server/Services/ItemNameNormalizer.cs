namespace Cooklyn.Server.Services;

using System.Text.RegularExpressions;

public static partial class ItemNameNormalizer
{
    private static readonly HashSet<string> Qualifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        "fresh", "dried", "organic", "whole", "large", "small", "medium",
        "frozen", "canned", "boneless", "skinless", "diced", "sliced",
        "minced", "chopped", "shredded", "extra", "finely", "roughly",
        "thinly", "raw", "cooked", "ripe", "peeled", "seeded", "pitted",
        "trimmed", "baby", "mini", "jumbo"
    };

    public static string Normalize(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        // Lowercase and trim
        var result = name.Trim().ToLowerInvariant();

        // Strip parentheticals
        result = ParentheticalRegex().Replace(result, "").Trim();

        // Split into words, remove qualifiers, rejoin
        var words = result.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var filtered = words.Where(w => !Qualifiers.Contains(w)).ToArray();

        // If all words were qualifiers, keep the original words
        if (filtered.Length == 0)
            filtered = words;

        // Basic singularize each word
        for (var i = 0; i < filtered.Length; i++)
            filtered[i] = BasicSingularize(filtered[i]);

        return string.Join(" ", filtered);
    }

    public static string[] ExtractTokens(string normalizedName)
    {
        if (string.IsNullOrWhiteSpace(normalizedName))
            return [];

        return normalizedName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string BasicSingularize(string word)
    {
        if (word.Length < 3)
            return word;

        // ies -> y (e.g. berries -> berry)
        if (word.EndsWith("ies", StringComparison.Ordinal))
            return word[..^3] + "y";

        // oes -> o (e.g. tomatoes -> tomato, potatoes -> potato)
        if (word.EndsWith("oes", StringComparison.Ordinal))
            return word[..^2];

        // Remove trailing s, but not from words ending in ss or us
        if (word.EndsWith('s') && !word.EndsWith("ss", StringComparison.Ordinal) && !word.EndsWith("us", StringComparison.Ordinal))
            return word[..^1];

        return word;
    }

    [GeneratedRegex(@"\([^)]*\)")]
    private static partial Regex ParentheticalRegex();
}
