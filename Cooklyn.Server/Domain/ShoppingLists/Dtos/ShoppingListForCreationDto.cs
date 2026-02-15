namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

public sealed record ShoppingListForCreationDto
{
    public string Name { get; init; } = default!;
    public string? StoreId { get; init; }
}
