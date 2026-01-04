namespace Cooklyn.Server.Domain.Tags.Dtos;

public sealed record TagDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Name { get; init; } = default!;
}
