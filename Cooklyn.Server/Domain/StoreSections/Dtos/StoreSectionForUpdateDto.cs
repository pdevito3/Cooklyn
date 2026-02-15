namespace Cooklyn.Server.Domain.StoreSections.Dtos;

public sealed record StoreSectionForUpdateDto
{
    public string Name { get; init; } = default!;
}
