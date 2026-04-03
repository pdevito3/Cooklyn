namespace Cooklyn.Server.Domain.Recipes.Mappings;

using BlobStorageKeys;
using Dtos;
using Models;
using Riok.Mapperly.Abstractions;
using Services;

[Mapper]
public static partial class RecipeMapper
{
    [MapperIgnoreSource(nameof(Recipe.IsDeleted))]
    [MapperIgnoreSource(nameof(Recipe.DomainEvents))]
    [MapperIgnoreSource(nameof(Recipe.RecipeTags))]
    [MapperIgnoreSource(nameof(Recipe.HasImage))]
    [MapperIgnoreTarget(nameof(RecipeDto.ImageUrl))]
    [MapProperty(nameof(Recipe.RecipeTags), nameof(RecipeDto.Tags))]
    [MapProperty(nameof(Recipe.Flags), nameof(RecipeDto.Flags))]
    [MapProperty(nameof(Recipe.Ingredients), nameof(RecipeDto.Ingredients))]
    private static partial RecipeDto ToRecipeDtoInternal(this Recipe recipe);

    /// <summary>
    /// Maps a Recipe to RecipeDto, including generating a presigned URL for the image.
    /// </summary>
    public static RecipeDto ToRecipeDto(this Recipe recipe, IFileStorage fileStorage)
    {
        var dto = recipe.ToRecipeDtoInternal();
        return dto with { ImageUrl = recipe.GetImagePreSignedUrl(fileStorage) };
    }

    [MapperIgnoreSource(nameof(Recipe.IsDeleted))]
    [MapperIgnoreSource(nameof(Recipe.DomainEvents))]
    [MapperIgnoreSource(nameof(Recipe.CreatedOn))]
    [MapperIgnoreSource(nameof(Recipe.LastModifiedOn))]
    [MapperIgnoreSource(nameof(Recipe.NutritionInfo))]
    [MapperIgnoreSource(nameof(Recipe.RecipeTags))]
    [MapperIgnoreSource(nameof(Recipe.Source))]
    [MapperIgnoreSource(nameof(Recipe.Steps))]
    [MapperIgnoreSource(nameof(Recipe.Notes))]
    [MapperIgnoreSource(nameof(Recipe.ImageS3Bucket))]
    [MapperIgnoreSource(nameof(Recipe.ImageS3Key))]
    [MapperIgnoreSource(nameof(Recipe.HasImage))]
    [MapperIgnoreSource(nameof(Recipe.Ingredients))]
    [MapperIgnoreTarget(nameof(RecipeSummaryDto.ImageUrl))]
    [MapperIgnoreTarget(nameof(RecipeSummaryDto.IngredientCount))]
    [MapProperty(nameof(Recipe.RecipeTags), nameof(RecipeSummaryDto.Tags))]
    [MapProperty(nameof(Recipe.Flags), nameof(RecipeSummaryDto.Flags))]
    private static partial RecipeSummaryDto ToRecipeSummaryDtoInternal(this Recipe recipe);

    /// <summary>
    /// Maps a Recipe to RecipeSummaryDto, including generating a presigned URL for the image.
    /// </summary>
    public static RecipeSummaryDto ToRecipeSummaryDto(this Recipe recipe, IFileStorage fileStorage)
    {
        var dto = recipe.ToRecipeSummaryDtoInternal();
        return dto with
        {
            ImageUrl = recipe.GetImagePreSignedUrl(fileStorage),
            IngredientCount = recipe.Ingredients.Count
        };
    }

    /// <summary>
    /// Converts an IQueryable of Recipes to RecipeSummaryDto. Note: ImageUrl will be null.
    /// Use ToRecipeSummaryDtoWithImageUrl extension after materializing for presigned URLs.
    /// </summary>
    public static partial IQueryable<RecipeSummaryDto> ToRecipeSummaryDtoQueryable(this IQueryable<Recipe> queryable);

    /// <summary>
    /// Adds presigned image URLs to a list of RecipeSummaryDto.
    /// </summary>
    public static IReadOnlyList<RecipeSummaryDto> WithImageUrls(
        this IEnumerable<RecipeSummaryDto> dtos,
        IEnumerable<Recipe> recipes,
        IFileStorage fileStorage)
    {
        var recipeDict = recipes.ToDictionary(r => r.Id);
        return dtos.Select(dto =>
        {
            if (recipeDict.TryGetValue(dto.Id, out var recipe))
            {
                return dto with { ImageUrl = recipe.GetImagePreSignedUrl(fileStorage) };
            }
            return dto;
        }).ToList();
    }

    public static RecipeForCreation ToRecipeForCreation(this RecipeForCreationDto dto)
    {
        return new RecipeForCreation
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageS3Bucket = dto.ImageS3Bucket,
            ImageS3Key = dto.ImageS3Key,
            Rating = dto.Rating,
            Source = dto.Source,
            Servings = dto.Servings,
            Steps = dto.Steps,
            Notes = dto.Notes
        };
    }

    public static partial RecipeForUpdate ToRecipeForUpdate(this RecipeForUpdateDto dto);

    public static IngredientForCreation ToIngredientForCreation(this IngredientForCreationDto dto, string recipeId)
    {
        return new IngredientForCreation
        {
            RecipeId = recipeId,
            RawText = dto.RawText,
            Name = dto.Name,
            Amount = dto.Amount,
            AmountText = dto.AmountText,
            Unit = dto.Unit,
            CustomUnit = dto.CustomUnit,
            GroupName = dto.GroupName,
            SortOrder = dto.SortOrder
        };
    }

    public static partial IngredientForUpdate ToIngredientForUpdate(this IngredientForCreationDto dto);

    // Value object mappings
    private static string MapRating(Rating rating) => rating.Value;

    private static string? MapBlobStorageKey(BlobStorageKey key) => key.Value;

    private static IReadOnlyList<string> MapRecipeTagsToTags(IReadOnlyCollection<RecipeTag> recipeTags)
        => recipeTags.Select(rt => rt.Tag.Name).ToList();

    private static IReadOnlyList<string> MapFlagsToStrings(IReadOnlyCollection<RecipeFlagEntry> flags)
        => flags.Select(f => f.Flag.Value).ToList();

    private static IReadOnlyList<IngredientDto> MapIngredientsToDto(IReadOnlyCollection<Ingredient> ingredients)
        => ingredients.OrderBy(i => i.SortOrder).Select(i => new IngredientDto
        {
            Id = i.Id,
            RawText = i.RawText,
            Name = i.Name,
            Amount = i.Amount,
            AmountText = i.AmountText,
            Unit = i.Unit.Value,
            CustomUnit = i.CustomUnit,
            GroupName = i.GroupName,
            SortOrder = i.SortOrder
        }).ToList();
}
