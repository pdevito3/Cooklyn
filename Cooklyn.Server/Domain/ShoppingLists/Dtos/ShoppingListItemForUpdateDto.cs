namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

public sealed record ShoppingListItemForUpdateDto
{
    public string Name { get; init; } = default!;
    public decimal? Quantity { get; init; }
    public string? Unit { get; init; }
    public string? StoreSectionId { get; init; }
    public string? Notes { get; init; }
}
