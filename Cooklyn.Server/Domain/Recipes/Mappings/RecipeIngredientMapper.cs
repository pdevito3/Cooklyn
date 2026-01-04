namespace Cooklyn.Server.Domain.Recipes.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class RecipeIngredientMapper
{
    [MapperIgnoreSource(nameof(RecipeIngredient.CreatedBy))]
    [MapperIgnoreSource(nameof(RecipeIngredient.LastModifiedBy))]
    [MapperIgnoreSource(nameof(RecipeIngredient.CreatedOn))]
    [MapperIgnoreSource(nameof(RecipeIngredient.LastModifiedOn))]
    [MapperIgnoreSource(nameof(RecipeIngredient.IsDeleted))]
    [MapperIgnoreSource(nameof(RecipeIngredient.DomainEvents))]
    public static partial RecipeIngredientDto ToRecipeIngredientDto(this RecipeIngredient ingredient);

    public static RecipeIngredientForCreation ToRecipeIngredientForCreation(
        this RecipeIngredientForCreationDto dto,
        Guid recipeId)
    {
        return new RecipeIngredientForCreation
        {
            RecipeId = recipeId,
            Name = dto.Name,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            SortOrder = dto.SortOrder,
            Notes = dto.Notes
        };
    }

    public static partial RecipeIngredientForUpdate ToRecipeIngredientForUpdate(
        this RecipeIngredientForUpdateDto dto);

    // Value object mappings
    private static string? MapUnit(UnitOfMeasure unit) => unit.IsEmpty ? null : unit.Value;
    private static string MapDisplayText(RecipeIngredient ingredient) => ingredient.GetDisplayText();
}
