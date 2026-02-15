namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

public sealed record ShoppingListItemDto
{
    public string Id { get; init; } = default!;
    public string ShoppingListId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public string? StoreSectionId { get; init; }
    public bool IsChecked { get; init; }
    public DateTimeOffset? CheckedOn { get; init; }
    public string? Notes { get; init; }
    public int SortOrder { get; init; }
    public IReadOnlyList<ShoppingListItemRecipeSourceDto> RecipeSources { get; init; } = [];
}
