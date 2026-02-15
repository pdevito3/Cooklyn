namespace Cooklyn.Server.Domain.Stores.Models;

public sealed record StoreForUpdate
{
    public string Name { get; init; } = default!;
    public string? Address { get; init; }
}
