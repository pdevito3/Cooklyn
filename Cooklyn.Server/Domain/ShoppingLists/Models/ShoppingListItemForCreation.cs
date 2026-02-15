namespace Cooklyn.Server.Domain.ShoppingLists.Models;

public sealed record ShoppingListItemForCreation
{
    public string ShoppingListId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public string? StoreSectionId { get; init; }
    public string? Notes { get; init; }
    public int SortOrder { get; init; }
}
