namespace Cooklyn.Server.Domain.SavedFilters.Dtos;

public sealed record SavedFilterForUpdateDto
{
    public string Name { get; init; } = default!;
    public string FilterStateJson { get; init; } = default!;
}
