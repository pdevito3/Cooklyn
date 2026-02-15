namespace Cooklyn.Server.Domain.ItemCollections.Models;

public sealed record ItemCollectionForUpdate
{
    public string Name { get; init; } = default!;
}
