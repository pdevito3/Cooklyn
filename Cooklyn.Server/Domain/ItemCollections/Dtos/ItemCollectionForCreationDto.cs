namespace Cooklyn.Server.Domain.ItemCollections.Dtos;

public sealed record ItemCollectionForCreationDto
{
    public string Name { get; init; } = default!;
}
