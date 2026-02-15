namespace Cooklyn.Server.Domain.Stores.Dtos;

public sealed record StoreAisleDto
{
    public string Id { get; init; } = default!;
    public string StoreId { get; init; } = default!;
    public string StoreSectionId { get; init; } = default!;
    public int SortOrder { get; init; }
    public string? CustomName { get; init; }
}
