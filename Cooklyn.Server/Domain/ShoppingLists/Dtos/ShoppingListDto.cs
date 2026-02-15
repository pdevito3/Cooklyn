namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

public sealed record ShoppingListDto
{
    public string Id { get; init; } = default!;
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? StoreId { get; init; }
    public string Status { get; init; } = default!;
    public DateTimeOffset? CompletedOn { get; init; }
    public IReadOnlyList<ShoppingListItemDto> Items { get; init; } = [];
}
