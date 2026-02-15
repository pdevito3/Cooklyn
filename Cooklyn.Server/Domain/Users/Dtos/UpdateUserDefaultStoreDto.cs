namespace Cooklyn.Server.Domain.Users.Dtos;

public sealed record UpdateUserDefaultStoreDto
{
    public string? StoreId { get; init; }
}
