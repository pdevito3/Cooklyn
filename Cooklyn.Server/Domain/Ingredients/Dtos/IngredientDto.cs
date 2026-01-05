namespace Cooklyn.Server.Domain.Ingredients.Dtos;

public sealed record IngredientDto
{
    public Guid Id { get; init; }
    public Guid RecipeId { get; init; }
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public int SortOrder { get; init; }
    public string? Notes { get; init; }
    public string DisplayText { get; init; } = default!;
}
