namespace Cooklyn.Server.Domain.Tenants.Dtos;

public sealed record TenantForCreationDto
{
    public string Name { get; init; } = default!;
}
