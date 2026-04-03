namespace Cooklyn.Server.Domain.ItemCollections.Dtos;

public sealed record ItemCollectionDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public IReadOnlyList<ItemCollectionItemDto> Items { get; init; } = [];
}
