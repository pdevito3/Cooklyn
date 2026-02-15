namespace Cooklyn.Server.Domain.Stores.Dtos;

public sealed record UpdateStoreDefaultCollectionsDto
{
    public IReadOnlyList<string> ItemCollectionIds { get; init; } = [];
}
