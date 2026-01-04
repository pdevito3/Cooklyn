namespace Cooklyn.Server.Domain.Tags.Dtos;

public sealed record TagForUpdateDto
{
    public string Name { get; init; } = default!;
}
