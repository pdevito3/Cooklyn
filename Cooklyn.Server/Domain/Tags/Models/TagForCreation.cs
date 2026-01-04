namespace Cooklyn.Server.Domain.Tags.Models;

public sealed record TagForCreation
{
    public Guid TenantId { get; init; }
    public string Name { get; init; } = default!;
}
