namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

public sealed record ShoppingListItemRecipeSourceDto
{
    public string Id { get; init; } = default!;
    public string RecipeId { get; init; } = default!;
    public decimal? OriginalQuantity { get; init; }
    public string? OriginalUnit { get; init; }
}
