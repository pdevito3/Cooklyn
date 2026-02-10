namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record IngredientDto
{
    public string Id { get; init; } = default!;
    public string RawText { get; init; } = default!;
    public string? Name { get; init; }
    public decimal? Amount { get; init; }
    public string? AmountText { get; init; }
    public string? Unit { get; init; }
    public string? CustomUnit { get; init; }
    public string? GroupName { get; init; }
    public int SortOrder { get; init; }
}
