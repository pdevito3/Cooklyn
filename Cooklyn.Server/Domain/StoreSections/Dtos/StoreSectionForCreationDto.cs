namespace Cooklyn.Server.Domain.StoreSections.Dtos;

public sealed record StoreSectionForCreationDto
{
    public string Name { get; init; } = default!;
}
