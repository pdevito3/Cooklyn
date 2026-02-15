namespace Cooklyn.Server.Domain.ItemCollections.Dtos;

public sealed record ItemCollectionForUpdateDto
{
    public string Name { get; init; } = default!;
}
