namespace Cooklyn.Server.Domain.Recipes;

using BlobStorageKeys;
using Exceptions;
using Recipes.DomainEvents;
using Recipes.Models;
using Services;
using Tags;

public class Recipe : BaseEntity, ITenantable
{
    public Guid TenantId { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? ImageS3Bucket { get; private set; }
    public BlobStorageKey ImageS3Key { get; private set; } = BlobStorageKey.Empty();
    public Rating Rating { get; private set; } = default!;
    public string? Source { get; private set; }
    public bool IsFavorite { get; private set; }
    public int? Servings { get; private set; }
    public string? Steps { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties with encapsulated collections
    private readonly List<RecipeTag> _recipeTags = [];
    public IReadOnlyCollection<RecipeTag> RecipeTags => _recipeTags.AsReadOnly();

    private readonly List<RecipeFlagEntry> _flags = [];
    public IReadOnlyCollection<RecipeFlagEntry> Flags => _flags.AsReadOnly();

    private readonly List<Ingredient> _ingredients = [];
    public IReadOnlyCollection<Ingredient> Ingredients => _ingredients.AsReadOnly();

    public NutritionInfo? NutritionInfo { get; private set; }

    public static Recipe Create(RecipeForCreation forCreation)
    {
        var recipe = new Recipe
        {
            TenantId = forCreation.TenantId,
            Title = forCreation.Title,
            Description = forCreation.Description,
            ImageS3Bucket = forCreation.ImageS3Bucket,
            ImageS3Key = BlobStorageKey.Of(forCreation.ImageS3Key),
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
        ImageS3Bucket = forUpdate.ImageS3Bucket;
        ImageS3Key = BlobStorageKey.Of(forUpdate.ImageS3Key);
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

    public Recipe SetImage(string? bucket, string? key)
    {
        ImageS3Bucket = bucket;
        ImageS3Key = BlobStorageKey.Of(key);
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public Recipe ClearImage()
    {
        ImageS3Bucket = null;
        ImageS3Key = BlobStorageKey.Empty();
        QueueDomainEvent(new RecipeUpdated(Id));
        return this;
    }

    public bool HasImage => !string.IsNullOrEmpty(ImageS3Bucket) && !ImageS3Key.IsEmpty;

    public string? GetImagePreSignedUrl(IFileStorage fileStorage, int durationInMinutes = 5)
    {
        if (!HasImage)
            return null;

        return fileStorage.GetPreSignedUrl(ImageS3Bucket!, ImageS3Key.Value!, durationInMinutes);
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

    // Ingredient management
    public Recipe SetIngredients(IEnumerable<Ingredient> ingredients)
    {
        _ingredients.Clear();
        _ingredients.AddRange(ingredients);
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
