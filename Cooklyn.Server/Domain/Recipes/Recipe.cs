namespace Cooklyn.Server.Domain.Recipes;

using Exceptions;
using Ingredients;
using Ingredients.Models;
using Recipes.DomainEvents;
using Recipes.Models;
using Tags;

public class Recipe : BaseEntity, ITenantable
{
    public Guid TenantId { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public Rating Rating { get; private set; } = default!;
    public string? Source { get; private set; }
    public bool IsFavorite { get; private set; }
    public int? Servings { get; private set; }
    public string? Steps { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties with encapsulated collections
    private readonly List<Ingredient> _ingredients = [];
    public IReadOnlyCollection<Ingredient> Ingredients => _ingredients.AsReadOnly();

    private readonly List<RecipeTag> _recipeTags = [];
    public IReadOnlyCollection<RecipeTag> RecipeTags => _recipeTags.AsReadOnly();

    private readonly List<RecipeFlagEntry> _flags = [];
    public IReadOnlyCollection<RecipeFlagEntry> Flags => _flags.AsReadOnly();

    public NutritionInfo? NutritionInfo { get; private set; }

    public static Recipe Create(RecipeForCreation forCreation)
    {
        var recipe = new Recipe
        {
            TenantId = forCreation.TenantId,
            Title = forCreation.Title,
            Description = forCreation.Description,
            ImageUrl = forCreation.ImageUrl,
            Rating = Rating.Of(forCreation.Rating ?? Rating.NotRated().Value),
            Source = forCreation.Source,
            IsFavorite = forCreation.IsFavorite,
            Servings = forCreation.Servings,
            Steps = forCreation.Steps,
            Notes = forCreation.Notes
        };

        ValidateRecipe(recipe);
        recipe.QueueDomainEvent(new RecipeCreated(recipe));

        return recipe;
    }

    public Recipe Update(RecipeForUpdate forUpdate)
    {
        Title = forUpdate.Title;
        Description = forUpdate.Description;
        ImageUrl = forUpdate.ImageUrl;
        Rating = Rating.Of(forUpdate.Rating ?? Rating.NotRated().Value);
        Source = forUpdate.Source;
        IsFavorite = forUpdate.IsFavorite;
        Servings = forUpdate.Servings;
        Steps = forUpdate.Steps;
        Notes = forUpdate.Notes;

        ValidateRecipe(this);
        QueueDomainEvent(new RecipeUpdated(Id));

        return this;
    }

    public Recipe SetImageUrl(string? imageUrl)
    {
        ImageUrl = imageUrl;
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe SetFavorite(bool isFavorite)
    {
        IsFavorite = isFavorite;
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe UpdateRating(Rating rating)
    {
        Rating = rating;
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    // Ingredient management
    public Ingredient AddIngredient(IngredientForCreation forCreation)
    {
        var ingredient = Ingredient.Create(forCreation with { RecipeId = Id });
        _ingredients.Add(ingredient);
        QueueDomainEvent(new RecipeUpdated(Id));
        return ingredient;
    }

    public Recipe RemoveIngredient(Ingredient ingredient)
    {
        _ingredients.RemoveAll(i => i.Id == ingredient.Id);
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe ReorderIngredients(IEnumerable<Guid> ingredientIds)
    {
        var orderedIds = ingredientIds.ToList();
        for (var i = 0; i < orderedIds.Count; i++)
        {
            var ingredient = _ingredients.FirstOrDefault(ing => ing.Id == orderedIds[i]);
            ingredient?.UpdateSortOrder(i);
        }
        return this;
    }

    // Tag management
    public RecipeTag AddTag(Tag tag)
    {
        if (_recipeTags.Any(rt => rt.TagId == tag.Id))
            return _recipeTags.First(rt => rt.TagId == tag.Id);

        var recipeTag = RecipeTag.Create(this, tag);
        _recipeTags.Add(recipeTag);
        QueueDomainEvent(new RecipeUpdated(Id));
        return recipeTag;
    }

    public Recipe RemoveTag(Tag tag)
    {
        _recipeTags.RemoveAll(rt => rt.TagId == tag.Id);
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe SetTags(IEnumerable<Tag> tags)
    {
        _recipeTags.Clear();
        foreach (var tag in tags)
        {
            _recipeTags.Add(RecipeTag.Create(this, tag));
        }
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    // Flag management
    public RecipeFlagEntry AddFlag(RecipeFlag flag)
    {
        if (_flags.Any(f => f.Flag.Value == flag.Value))
            return _flags.First(f => f.Flag.Value == flag.Value);

        var entry = RecipeFlagEntry.Create(Id, flag);
        _flags.Add(entry);
        QueueDomainEvent(new RecipeUpdated(Id));
        return entry;
    }

    public Recipe RemoveFlag(RecipeFlag flag)
    {
        _flags.RemoveAll(f => f.Flag.Value == flag.Value);
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe SetFlags(IEnumerable<RecipeFlag> flags)
    {
        _flags.Clear();
        foreach (var flag in flags)
        {
            _flags.Add(RecipeFlagEntry.Create(Id, flag));
        }
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public bool HasFlag(RecipeFlag flag) => _flags.Any(f => f.Flag.Value == flag.Value);

    // Nutrition info management
    public Recipe SetNutritionInfo(NutritionInfoForCreation forCreation)
    {
        NutritionInfo = NutritionInfo.Create(forCreation with { RecipeId = Id });
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe UpdateNutritionInfo(NutritionInfoForUpdate forUpdate)
    {
        NutritionInfo?.Update(forUpdate);
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe ClearNutritionInfo()
    {
        NutritionInfo = null;
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    private static void ValidateRecipe(Recipe recipe)
    {
        ValidationException.ThrowWhenEmpty(recipe.TenantId, "Please provide a tenant.");
        ValidationException.ThrowWhenNullOrWhitespace(recipe.Title, "Please provide a recipe title.");
    }

    protected Recipe() { } // EF Core
}
