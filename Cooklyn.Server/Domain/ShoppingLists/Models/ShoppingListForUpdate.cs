namespace Cooklyn.Server.Domain.ShoppingLists.Models;

public sealed record ShoppingListForUpdate
{
    public string Name { get; init; } = default!;
    public string? StoreId { get; init; }
}
