namespace Cooklyn.Server.Domain.StoreSections.Models;

public sealed record StoreSectionForCreation
{
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
}
