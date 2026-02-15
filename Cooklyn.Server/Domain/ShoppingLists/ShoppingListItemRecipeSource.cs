namespace Cooklyn.Server.Domain.ShoppingLists;

using Recipes;

public class ShoppingListItemRecipeSource : BaseEntity
{
    public string ShoppingListItemId { get; private set; } = default!;
    public string RecipeId { get; private set; } = default!;
    public decimal? OriginalQuantity { get; private set; }
    public string? OriginalUnit { get; private set; }

    public static ShoppingListItemRecipeSource Create(
        string shoppingListItemId,
        string recipeId,
        decimal? originalQuantity,
        string? originalUnit)
    {
        return new ShoppingListItemRecipeSource
        {
            ShoppingListItemId = shoppingListItemId,
            RecipeId = recipeId,
            OriginalQuantity = originalQuantity,
            OriginalUnit = originalUnit
        };
    }

    protected ShoppingListItemRecipeSource() { } // EF Core
}
