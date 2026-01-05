namespace Cooklyn.Server.Domain.Ingredients.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class IngredientMapper
{
    [MapperIgnoreSource(nameof(Ingredient.CreatedBy))]
    [MapperIgnoreSource(nameof(Ingredient.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Ingredient.CreatedOn))]
    [MapperIgnoreSource(nameof(Ingredient.LastModifiedOn))]
    [MapperIgnoreSource(nameof(Ingredient.IsDeleted))]
    [MapperIgnoreSource(nameof(Ingredient.DomainEvents))]
    public static partial IngredientDto ToIngredientDto(this Ingredient ingredient);

    public static partial IQueryable<IngredientDto> ToIngredientDtoQueryable(this IQueryable<Ingredient> queryable);

    public static IngredientForCreation ToIngredientForCreation(
        this IngredientForCreationDto dto,
        Guid recipeId,
        int sortOrder)
    {
        return new IngredientForCreation
        {
            RecipeId = recipeId,
            Name = dto.Name,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            SortOrder = dto.SortOrder > 0 ? dto.SortOrder : sortOrder,
            Notes = dto.Notes
        };
    }

    public static partial IngredientForUpdate ToIngredientForUpdate(this IngredientForUpdateDto dto);

    // Value object mappings
    private static string? MapUnit(UnitOfMeasure unit) => unit.IsEmpty ? null : unit.Value;
    private static string MapDisplayText(Ingredient ingredient) => ingredient.GetDisplayText();
}
