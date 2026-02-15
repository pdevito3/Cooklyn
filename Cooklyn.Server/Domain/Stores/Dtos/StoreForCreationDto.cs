namespace Cooklyn.Server.Domain.Stores.Dtos;

public sealed record StoreForCreationDto
{
    public string Name { get; init; } = default!;
    public string? Address { get; init; }
}
