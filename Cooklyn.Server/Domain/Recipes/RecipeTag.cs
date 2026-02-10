namespace Cooklyn.Server.Domain.Recipes;

using Tags;

public class RecipeTag : BaseEntity
{
    public string RecipeId { get; private set; } = default!;
    public Recipe Recipe { get; private set; } = default!;
    public string TagId { get; private set; } = default!;
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
