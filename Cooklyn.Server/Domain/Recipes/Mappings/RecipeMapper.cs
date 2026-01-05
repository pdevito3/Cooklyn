namespace Cooklyn.Server.Domain.Recipes.Mappings;

using Dtos;
using Ingredients;
using Ingredients.Dtos;
using Ingredients.Mappings;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class RecipeMapper
{
    [MapperIgnoreSource(nameof(Recipe.CreatedBy))]
    [MapperIgnoreSource(nameof(Recipe.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Recipe.IsDeleted))]
    [MapperIgnoreSource(nameof(Recipe.DomainEvents))]
    [MapperIgnoreSource(nameof(Recipe.RecipeTags))]
    [MapProperty(nameof(Recipe.RecipeTags), nameof(RecipeDto.Tags))]
    [MapProperty(nameof(Recipe.Flags), nameof(RecipeDto.Flags))]
    public static partial RecipeDto ToRecipeDto(this Recipe recipe);

    [MapperIgnoreSource(nameof(Recipe.CreatedBy))]
    [MapperIgnoreSource(nameof(Recipe.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Recipe.IsDeleted))]
    [MapperIgnoreSource(nameof(Recipe.DomainEvents))]
    [MapperIgnoreSource(nameof(Recipe.CreatedOn))]
    [MapperIgnoreSource(nameof(Recipe.LastModifiedOn))]
    [MapperIgnoreSource(nameof(Recipe.Ingredients))]
    [MapperIgnoreSource(nameof(Recipe.NutritionInfo))]
    [MapperIgnoreSource(nameof(Recipe.RecipeTags))]
    [MapperIgnoreSource(nameof(Recipe.Source))]
    [MapperIgnoreSource(nameof(Recipe.Steps))]
    [MapperIgnoreSource(nameof(Recipe.Notes))]
    [MapperIgnoreSource(nameof(Recipe.TenantId))]
    [MapProperty(nameof(Recipe.RecipeTags), nameof(RecipeSummaryDto.Tags))]
    [MapProperty(nameof(Recipe.Flags), nameof(RecipeSummaryDto.Flags))]
    public static partial RecipeSummaryDto ToRecipeSummaryDto(this Recipe recipe);

    public static partial IQueryable<RecipeSummaryDto> ToRecipeSummaryDtoQueryable(this IQueryable<Recipe> queryable);

    public static RecipeForCreation ToRecipeForCreation(this RecipeForCreationDto dto, Guid tenantId)
    {
        return new RecipeForCreation
        {
            TenantId = tenantId,
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
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

    private static IReadOnlyList<string> MapRecipeTagsToTags(IReadOnlyCollection<RecipeTag> recipeTags)
        => recipeTags.Select(rt => rt.Tag.Name).ToList();

    private static IReadOnlyList<string> MapFlagsToStrings(IReadOnlyCollection<RecipeFlagEntry> flags)
        => flags.Select(f => f.Flag.Value).ToList();
}
