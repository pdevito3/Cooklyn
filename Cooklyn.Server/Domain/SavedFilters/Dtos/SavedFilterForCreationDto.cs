namespace Cooklyn.Server.Domain.SavedFilters.Dtos;

public sealed record SavedFilterForCreationDto
{
    public string Name { get; init; } = default!;
    public string Context { get; init; } = default!;
    public string FilterStateJson { get; init; } = default!;
}
