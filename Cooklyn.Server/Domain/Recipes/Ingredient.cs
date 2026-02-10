namespace Cooklyn.Server.Domain.Recipes;

using System.Text.RegularExpressions;
using Dtos;
using Models;

public partial class Ingredient : BaseEntity
{
    public Guid RecipeId { get; private set; }
    public string RawText { get; private set; } = default!;
    public string? Name { get; private set; }
    public decimal? Amount { get; private set; }
    public string? AmountText { get; private set; }
    public IngredientUnit Unit { get; private set; } = new(string.Empty);
    public bool HasUnit => !string.IsNullOrEmpty(Unit.Value);
    public string? CustomUnit { get; private set; }
    public string? GroupName { get; private set; }
    public int SortOrder { get; private set; }

    public static Ingredient Create(IngredientForCreation forCreation)
    {
        return new Ingredient
        {
            RecipeId = forCreation.RecipeId,
            RawText = forCreation.RawText,
            Name = forCreation.Name,
            Amount = forCreation.Amount,
            AmountText = forCreation.AmountText,
            Unit = new IngredientUnit(forCreation.Unit ?? string.Empty),
            CustomUnit = forCreation.CustomUnit,
            GroupName = forCreation.GroupName,
            SortOrder = forCreation.SortOrder
        };
    }

    public Ingredient Update(IngredientForUpdate forUpdate)
    {
        RawText = forUpdate.RawText;
        Name = forUpdate.Name;
        Amount = forUpdate.Amount;
        AmountText = forUpdate.AmountText;
        Unit = new IngredientUnit(forUpdate.Unit ?? string.Empty);
        CustomUnit = forUpdate.CustomUnit;
        GroupName = forUpdate.GroupName;
        SortOrder = forUpdate.SortOrder;
        return this;
    }

    // --- Parser logic (absorbed from IngredientParser.cs) ---

    // Matches: "1 1/2", "1/2", "1.5", "1", etc. at the start of a line
    [GeneratedRegex(@"^(\d+\s+\d+/\d+|\d+/\d+|\d+\.?\d*)")]
    private static partial Regex AmountPattern();

    /// <summary>
    /// Parses a multi-line text block into structured Ingredient entities.
    /// Lines ending with ":" (e.g. "Biscuit:") are treated as group headers.
    /// </summary>
    public static IReadOnlyList<Ingredient> ParseAll(string text, Guid recipeId)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var lines = text.Split('\n', StringSplitOptions.TrimEntries);
        var results = new List<Ingredient>();
        var sortOrder = 0;
        string? currentGroup = null;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var groupName = ParseGroupHeader(line);
            if (groupName != null)
            {
                currentGroup = groupName;
                continue;
            }

            var ingredient = Parse(line, recipeId, sortOrder++, currentGroup);
            results.Add(ingredient);
        }

        return results;
    }

    private static string? ParseGroupHeader(string line)
    {
        var trimmed = line.Trim();
        if (trimmed.EndsWith(':') && trimmed.Length > 1 && !char.IsDigit(trimmed[0]))
            return trimmed[..^1].Trim();
        return null;
    }

    /// <summary>
    /// Parses a single ingredient line into a structured Ingredient entity.
    /// </summary>
    public static Ingredient Parse(string line, Guid recipeId, int sortOrder, string? groupName = null)
    {
        var trimmed = line.Trim();
        var remaining = trimmed;

        decimal? amount = null;
        string? amountText = null;
        IngredientUnit unit = new(string.Empty);
        string? name = null;

        // Try to extract amount from the beginning
        var amountMatch = AmountPattern().Match(remaining);
        if (amountMatch.Success)
        {
            amountText = amountMatch.Value.Trim();
            amount = ParseAmount(amountText);
            remaining = remaining[amountMatch.Length..].TrimStart();
        }

        // Try to match a unit from the next word(s)
        if (remaining.Length > 0 && amount.HasValue)
        {
            var unitMatch = TryMatchUnit(remaining);
            if (unitMatch != null)
            {
                unit = new IngredientUnit(unitMatch.Value.unitName);
                remaining = remaining[unitMatch.Value.consumed..].TrimStart();
            }
        }

        // Everything remaining is the ingredient name
        name = remaining.Length > 0 ? remaining : null;

        return Create(new IngredientForCreation
        {
            RecipeId = recipeId,
            RawText = trimmed,
            Name = name,
            Amount = amount,
            AmountText = amountText,
            Unit = unit.Value,
            GroupName = groupName,
            SortOrder = sortOrder
        });
    }

    private static decimal? ParseAmount(string text)
    {
        // Mixed fraction: "1 1/2"
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2 && TryParseFraction(parts[1], out var fractionPart) && decimal.TryParse(parts[0], out var wholePart))
        {
            return wholePart + fractionPart;
        }

        // Simple fraction: "1/2"
        if (TryParseFraction(text, out var fraction))
        {
            return fraction;
        }

        // Decimal or integer: "1.5", "2"
        if (decimal.TryParse(text, out var dec))
        {
            return dec;
        }

        return null;
    }

    private static bool TryParseFraction(string text, out decimal result)
    {
        result = 0;
        var slashIndex = text.IndexOf('/');
        if (slashIndex <= 0 || slashIndex >= text.Length - 1)
            return false;

        if (decimal.TryParse(text[..slashIndex], out var numerator) &&
            decimal.TryParse(text[(slashIndex + 1)..], out var denominator) &&
            denominator != 0)
        {
            result = numerator / denominator;
            return true;
        }

        return false;
    }

    private static (string unitName, int consumed)? TryMatchUnit(string text)
    {
        // Try multi-word units first (e.g. "fl oz", "fluid ounce")
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Try two-word match
        if (words.Length >= 2)
        {
            var twoWord = $"{words[0]} {words[1]}";
            var parsed = IngredientUnit.TryParse(twoWord);
            if (parsed != null)
            {
                return (parsed.Value, twoWord.Length);
            }
        }

        // Try single-word match (strip trailing period/comma)
        if (words.Length >= 1)
        {
            var firstWord = words[0].TrimEnd('.', ',');
            var parsed = IngredientUnit.TryParse(firstWord);
            if (parsed != null)
            {
                return (parsed.Value, words[0].Length);
            }
        }

        return null;
    }

    protected Ingredient() { } // EF Core
}
