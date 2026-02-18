namespace Cooklyn.Server.Domain.RecentSearches.Models;

public sealed record RecentSearchForCreation
{
    public string TenantId { get; init; } = default!;
    public string SearchType { get; init; } = default!;
    public string SearchText { get; init; } = default!;
    public string? ResourceType { get; init; }
    public string? ResourceId { get; init; }
}
