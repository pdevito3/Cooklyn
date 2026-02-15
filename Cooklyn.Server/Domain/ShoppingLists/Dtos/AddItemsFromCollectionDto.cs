namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

public sealed record AddItemsFromCollectionDto
{
    public string ItemCollectionId { get; init; } = default!;
}
