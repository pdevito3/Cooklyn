namespace Cooklyn.Server.Domain.RecentSearches.Dtos;

public sealed record RecentSearchDto
{
    public string Id { get; init; } = default!;
    public string SearchType { get; init; } = default!;
    public string SearchText { get; init; } = default!;
    public string? ResourceType { get; init; }
    public string? ResourceId { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
}
