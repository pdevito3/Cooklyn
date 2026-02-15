namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

public sealed record ShoppingListSummaryDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? StoreId { get; init; }
    public string Status { get; init; } = default!;
    public int ItemCount { get; init; }
    public int CheckedCount { get; init; }
    public DateTimeOffset? CompletedOn { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
}
