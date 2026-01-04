namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record RecipeIngredientForCreationDto
{
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public int SortOrder { get; init; }
    public string? Notes { get; init; }
}
