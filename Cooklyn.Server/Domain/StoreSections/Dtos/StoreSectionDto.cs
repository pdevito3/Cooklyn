namespace Cooklyn.Server.Domain.StoreSections.Dtos;

public sealed record StoreSectionDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
}
