namespace Cooklyn.Server.Domain.ShoppingLists.Models;

public sealed record ShoppingListForCreation
{
    public string Name { get; init; } = default!;
    public string? StoreId { get; init; }
}
