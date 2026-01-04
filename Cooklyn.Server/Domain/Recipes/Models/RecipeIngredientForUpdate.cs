namespace Cooklyn.Server.Domain.Recipes.Models;

public sealed record RecipeIngredientForUpdate
{
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public int SortOrder { get; init; }
    public string? Notes { get; init; }
}
