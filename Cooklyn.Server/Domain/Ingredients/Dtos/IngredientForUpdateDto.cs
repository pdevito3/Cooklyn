namespace Cooklyn.Server.Domain.Ingredients.Dtos;

public sealed record IngredientForUpdateDto
{
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public int SortOrder { get; init; }
    public string? Notes { get; init; }
}
