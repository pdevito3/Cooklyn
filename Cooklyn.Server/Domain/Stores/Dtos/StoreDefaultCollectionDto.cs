namespace Cooklyn.Server.Domain.Stores.Dtos;

public sealed record StoreDefaultCollectionDto
{
    public string Id { get; init; } = default!;
    public string ItemCollectionId { get; init; } = default!;
    public string ItemCollectionName { get; init; } = default!;
}
