namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

public sealed record AddItemsFromRecipeDto
{
    public string RecipeId { get; init; } = default!;
    public IReadOnlyList<string>? IngredientIds { get; init; }
}
