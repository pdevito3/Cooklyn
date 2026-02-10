namespace Cooklyn.Server.Domain.Tags.Models;

public sealed record TagForCreation
{
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
}
