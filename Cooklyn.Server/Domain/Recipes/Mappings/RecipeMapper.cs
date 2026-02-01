namespace Cooklyn.Server.Domain.Recipes.Mappings;

using BlobStorageKeys;
using Dtos;
using Models;
using Riok.Mapperly.Abstractions;
using Services;

[Mapper]
public static partial class RecipeMapper
{
    [MapperIgnoreSource(nameof(Recipe.CreatedBy))]
    [MapperIgnoreSource(nameof(Recipe.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Recipe.IsDeleted))]
    [MapperIgnoreSource(nameof(Recipe.DomainEvents))]
    [MapperIgnoreSource(nameof(Recipe.RecipeTags))]
    [MapperIgnoreSource(nameof(Recipe.HasImage))]
    [MapperIgnoreTarget(nameof(RecipeDto.ImageUrl))]
    [MapProperty(nameof(Recipe.RecipeTags), nameof(RecipeDto.Tags))]
    [MapProperty(nameof(Recipe.Flags), nameof(RecipeDto.Flags))]
    private static partial RecipeDto ToRecipeDtoInternal(this Recipe recipe);

    /// <summary>
    /// Maps a Recipe to RecipeDto, including generating a presigned URL for the image.
    /// </summary>
    public static RecipeDto ToRecipeDto(this Recipe recipe, IFileStorage fileStorage)
    {
        var dto = recipe.ToRecipeDtoInternal();
        return dto with { ImageUrl = recipe.GetImagePreSignedUrl(fileStorage) };
    }

    [MapperIgnoreSource(nameof(Recipe.CreatedBy))]
    [MapperIgnoreSource(nameof(Recipe.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Recipe.IsDeleted))]
    [MapperIgnoreSource(nameof(Recipe.DomainEvents))]
    [MapperIgnoreSource(nameof(Recipe.CreatedOn))]
    [MapperIgnoreSource(nameof(Recipe.LastModifiedOn))]
    [MapperIgnoreSource(nameof(Recipe.NutritionInfo))]
    [MapperIgnoreSource(nameof(Recipe.RecipeTags))]
    [MapperIgnoreSource(nameof(Recipe.Source))]
    [MapperIgnoreSource(nameof(Recipe.Steps))]
    [MapperIgnoreSource(nameof(Recipe.Notes))]
    [MapperIgnoreSource(nameof(Recipe.TenantId))]
    [MapperIgnoreSource(nameof(Recipe.ImageS3Bucket))]
    [MapperIgnoreSource(nameof(Recipe.ImageS3Key))]
    [MapperIgnoreSource(nameof(Recipe.HasImage))]
    [MapperIgnoreTarget(nameof(RecipeSummaryDto.ImageUrl))]
    [MapProperty(nameof(Recipe.RecipeTags), nameof(RecipeSummaryDto.Tags))]
    [MapProperty(nameof(Recipe.Flags), nameof(RecipeSummaryDto.Flags))]
    private static partial RecipeSummaryDto ToRecipeSummaryDtoInternal(this Recipe recipe);

    /// <summary>
    /// Maps a Recipe to RecipeSummaryDto, including generating a presigned URL for the image.
    /// </summary>
    public static RecipeSummaryDto ToRecipeSummaryDto(this Recipe recipe, IFileStorage fileStorage)
    {
        var dto = recipe.ToRecipeSummaryDtoInternal();
        return dto with { ImageUrl = recipe.GetImagePreSignedUrl(fileStorage) };
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

    public static RecipeForCreation ToRecipeForCreation(this RecipeForCreationDto dto, Guid tenantId)
    {
        return new RecipeForCreation
        {
            TenantId = tenantId,
            Title = dto.Title,
            Description = dto.Description,
            ImageS3Bucket = dto.ImageS3Bucket,
            ImageS3Key = dto.ImageS3Key,
            Rating = dto.Rating,
            Source = dto.Source,
            IsFavorite = dto.IsFavorite,
            Servings = dto.Servings,
            Steps = dto.Steps,
            Notes = dto.Notes
        };
    }

    public static partial RecipeForUpdate ToRecipeForUpdate(this RecipeForUpdateDto dto);

    // Value object mappings
    private static string MapRating(Rating rating) => rating.Value;

    private static string? MapBlobStorageKey(BlobStorageKey key) => key.Value;

    private static IReadOnlyList<string> MapRecipeTagsToTags(IReadOnlyCollection<RecipeTag> recipeTags)
        => recipeTags.Select(rt => rt.Tag.Name).ToList();

    private static IReadOnlyList<string> MapFlagsToStrings(IReadOnlyCollection<RecipeFlagEntry> flags)
        => flags.Select(f => f.Flag.Value).ToList();
}
