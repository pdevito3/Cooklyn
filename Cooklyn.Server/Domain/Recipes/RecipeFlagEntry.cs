namespace Cooklyn.Server.Domain.Recipes;

public class RecipeFlagEntry : BaseEntity
{
    public string RecipeId { get; private set; } = default!;
    public RecipeFlag Flag { get; private set; } = default!;

    public static RecipeFlagEntry Create(string recipeId, RecipeFlag flag)
    {
        return new RecipeFlagEntry
        {
            RecipeId = recipeId,
            Flag = flag
        };
    }

    protected RecipeFlagEntry() { } // EF Core
}
