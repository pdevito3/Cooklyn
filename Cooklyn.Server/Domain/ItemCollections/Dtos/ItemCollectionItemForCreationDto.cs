namespace Cooklyn.Server.Domain.ItemCollections.Dtos;

public sealed record ItemCollectionItemForCreationDto
{
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public string? StoreSectionId { get; init; }
    public int SortOrder { get; init; }
}
