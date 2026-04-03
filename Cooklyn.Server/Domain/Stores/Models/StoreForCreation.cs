namespace Cooklyn.Server.Domain.Stores.Models;

public sealed record StoreForCreation
{
    public string Name { get; init; } = default!;
    public string? Address { get; init; }
}
