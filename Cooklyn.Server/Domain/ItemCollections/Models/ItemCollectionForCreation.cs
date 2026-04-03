namespace Cooklyn.Server.Domain.ItemCollections.Models;

public sealed record ItemCollectionForCreation
{
    public string Name { get; init; } = default!;
}
