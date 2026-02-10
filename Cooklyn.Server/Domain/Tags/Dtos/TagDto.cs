namespace Cooklyn.Server.Domain.Tags.Dtos;

public sealed record TagDto
{
    public string Id { get; init; } = default!;
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
}
