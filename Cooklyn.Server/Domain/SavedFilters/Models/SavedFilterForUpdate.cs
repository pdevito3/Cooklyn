namespace Cooklyn.Server.Domain.SavedFilters.Models;

public sealed record SavedFilterForUpdate
{
    public string Name { get; init; } = default!;
    public string FilterStateJson { get; init; } = default!;
}
