namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record RecipeIngredientDto
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
