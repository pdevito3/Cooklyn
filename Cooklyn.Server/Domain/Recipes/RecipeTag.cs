namespace Cooklyn.Server.Domain.Recipes;

using Tags;

public class RecipeTag : BaseEntity
{
    public Guid RecipeId { get; private set; }
    public Recipe Recipe { get; private set; } = default!;
    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public static RecipeTag Create(Recipe recipe, Tag tag)
    {
        return new RecipeTag
        {
            RecipeId = recipe.Id,
            Recipe = recipe,
            TagId = tag.Id,
            Tag = tag
        };
    }

    protected RecipeTag() { } // EF Core
}
