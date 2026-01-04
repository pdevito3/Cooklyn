namespace Cooklyn.Server.Domain.Tags.Dtos;

public sealed record TagForCreationDto
{
    public string Name { get; init; } = default!;
}
