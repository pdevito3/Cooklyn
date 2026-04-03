namespace Cooklyn.Server.Domain.ItemCategoryMappings.Models;

public sealed record ItemCategoryMappingForCreation
{
    public string NormalizedName { get; init; } = default!;
    public string StoreSectionId { get; init; } = default!;
    public string Source { get; init; } = default!;
}
