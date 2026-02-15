namespace Cooklyn.Server.Domain.Stores.Models;

public sealed record StoreForCreation
{
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? Address { get; init; }
}
