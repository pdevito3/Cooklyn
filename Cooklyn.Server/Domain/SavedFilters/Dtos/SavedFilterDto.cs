namespace Cooklyn.Server.Domain.SavedFilters.Dtos;

public sealed record SavedFilterDto
{
    public string Id { get; init; } = default!;
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Context { get; init; } = default!;
    public string FilterStateJson { get; init; } = default!;
}
