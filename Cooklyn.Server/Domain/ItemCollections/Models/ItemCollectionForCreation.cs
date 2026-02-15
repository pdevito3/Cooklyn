namespace Cooklyn.Server.Domain.ItemCollections.Models;

public sealed record ItemCollectionForCreation
{
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
}
