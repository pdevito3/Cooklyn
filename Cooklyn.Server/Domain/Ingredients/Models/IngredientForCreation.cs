namespace Cooklyn.Server.Domain.Ingredients.Models;

public sealed record IngredientForCreation
{
    public Guid RecipeId { get; init; }
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public int SortOrder { get; init; }
    public string? Notes { get; init; }
}
