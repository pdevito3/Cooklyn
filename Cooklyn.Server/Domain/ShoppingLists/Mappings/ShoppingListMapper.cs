namespace Cooklyn.Server.Domain.ShoppingLists.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class ShoppingListMapper
{
    [MapperIgnoreSource(nameof(ShoppingList.CreatedOn))]
    [MapperIgnoreSource(nameof(ShoppingList.LastModifiedOn))]
    [MapperIgnoreSource(nameof(ShoppingList.IsDeleted))]
    [MapperIgnoreSource(nameof(ShoppingList.DomainEvents))]
    public static partial ShoppingListDto ToShoppingListDto(this ShoppingList shoppingList);

    [MapperIgnoreSource(nameof(ShoppingListItem.CreatedOn))]
    [MapperIgnoreSource(nameof(ShoppingListItem.LastModifiedOn))]
    [MapperIgnoreSource(nameof(ShoppingListItem.IsDeleted))]
    [MapperIgnoreSource(nameof(ShoppingListItem.DomainEvents))]
    public static partial ShoppingListItemDto ToShoppingListItemDto(this ShoppingListItem item);

    [MapperIgnoreSource(nameof(ShoppingListItemRecipeSource.CreatedOn))]
    [MapperIgnoreSource(nameof(ShoppingListItemRecipeSource.LastModifiedOn))]
    [MapperIgnoreSource(nameof(ShoppingListItemRecipeSource.IsDeleted))]
    [MapperIgnoreSource(nameof(ShoppingListItemRecipeSource.DomainEvents))]
    [MapperIgnoreSource(nameof(ShoppingListItemRecipeSource.ShoppingListItemId))]
    public static partial ShoppingListItemRecipeSourceDto ToRecipeSourceDto(this ShoppingListItemRecipeSource source);

    public static ShoppingListForCreation ToShoppingListForCreation(this ShoppingListForCreationDto dto)
    {
        return new ShoppingListForCreation
        {
            Name = dto.Name,
            StoreId = dto.StoreId
        };
    }

    public static partial ShoppingListForUpdate ToShoppingListForUpdate(this ShoppingListForUpdateDto dto);

    public static ShoppingListItemForCreation ToShoppingListItemForCreation(this ShoppingListItemForCreationDto dto, string shoppingListId, int sortOrder)
    {
        return new ShoppingListItemForCreation
        {
            ShoppingListId = shoppingListId,
            Name = dto.Name,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            StoreSectionId = dto.StoreSectionId,
            Notes = dto.Notes,
            SortOrder = sortOrder
        };
    }

    public static ShoppingListItemForUpdate ToShoppingListItemForUpdate(this ShoppingListItemForUpdateDto dto, int sortOrder)
    {
        return new ShoppingListItemForUpdate
        {
            Name = dto.Name,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            StoreSectionId = dto.StoreSectionId,
            Notes = dto.Notes,
            SortOrder = sortOrder
        };
    }

    private static string MapStatus(ShoppingListStatus status) => status.Value;
    private static string MapUnit(Recipes.IngredientUnit unit) => unit.Value;
}
