namespace Cooklyn.Server.Domain.SavedFilters.Models;

public sealed record SavedFilterForCreation
{
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Context { get; init; } = default!;
    public string FilterStateJson { get; init; } = default!;
}
