namespace Cooklyn.Server.Domain.Stores.Dtos;

public sealed record StoreAisleForUpdateDto
{
    public string StoreSectionId { get; init; } = default!;
    public int SortOrder { get; init; }
    public string? CustomName { get; init; }
}
