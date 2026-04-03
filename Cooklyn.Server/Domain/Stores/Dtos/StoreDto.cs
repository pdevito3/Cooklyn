namespace Cooklyn.Server.Domain.Stores.Dtos;

public sealed record StoreDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? Address { get; init; }
    public IReadOnlyList<StoreAisleDto> StoreAisles { get; init; } = [];
    public IReadOnlyList<StoreDefaultCollectionDto> StoreDefaultCollections { get; init; } = [];
}
