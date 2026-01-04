namespace Cooklyn.Server.Domain.Tenants.Models;

public sealed record TenantForUpdate
{
    public string Name { get; init; } = default!;
}
