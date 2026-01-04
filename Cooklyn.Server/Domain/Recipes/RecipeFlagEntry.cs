namespace Cooklyn.Server.Domain.Recipes;

public class RecipeFlagEntry : BaseEntity
{
    public Guid RecipeId { get; private set; }
    public RecipeFlag Flag { get; private set; } = default!;

    public static RecipeFlagEntry Create(Guid recipeId, RecipeFlag flag)
    {
        return new RecipeFlagEntry
        {
            RecipeId = recipeId,
            Flag = flag
        };
    }

    protected RecipeFlagEntry() { } // EF Core
}
