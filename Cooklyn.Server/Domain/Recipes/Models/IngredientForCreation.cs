namespace Cooklyn.Server.Domain.Recipes.Models;

public sealed record IngredientForCreation
{
    public string RecipeId { get; init; } = default!;
    public string RawText { get; init; } = default!;
    public string? Name { get; init; }
    public decimal? Amount { get; init; }
    public string? AmountText { get; init; }
    public string? Unit { get; init; }
    public string? CustomUnit { get; init; }
    public string? GroupName { get; init; }
    public int SortOrder { get; init; }
}
