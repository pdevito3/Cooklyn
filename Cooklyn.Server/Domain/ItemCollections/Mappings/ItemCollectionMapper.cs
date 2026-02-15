namespace Cooklyn.Server.Domain.ItemCollections.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class ItemCollectionMapper
{
    [MapperIgnoreSource(nameof(ItemCollection.CreatedBy))]
    [MapperIgnoreSource(nameof(ItemCollection.LastModifiedBy))]
    [MapperIgnoreSource(nameof(ItemCollection.CreatedOn))]
    [MapperIgnoreSource(nameof(ItemCollection.LastModifiedOn))]
    [MapperIgnoreSource(nameof(ItemCollection.IsDeleted))]
    [MapperIgnoreSource(nameof(ItemCollection.DomainEvents))]
    public static partial ItemCollectionDto ToItemCollectionDto(this ItemCollection collection);

    [MapperIgnoreSource(nameof(ItemCollectionItem.CreatedBy))]
    [MapperIgnoreSource(nameof(ItemCollectionItem.LastModifiedBy))]
    [MapperIgnoreSource(nameof(ItemCollectionItem.CreatedOn))]
    [MapperIgnoreSource(nameof(ItemCollectionItem.LastModifiedOn))]
    [MapperIgnoreSource(nameof(ItemCollectionItem.IsDeleted))]
    [MapperIgnoreSource(nameof(ItemCollectionItem.DomainEvents))]
    public static partial ItemCollectionItemDto ToItemCollectionItemDto(this ItemCollectionItem item);

    public static ItemCollectionForCreation ToItemCollectionForCreation(this ItemCollectionForCreationDto dto, string tenantId)
    {
        return new ItemCollectionForCreation
        {
            TenantId = tenantId,
            Name = dto.Name
        };
    }

    public static partial ItemCollectionForUpdate ToItemCollectionForUpdate(this ItemCollectionForUpdateDto dto);

    private static string MapUnit(Recipes.IngredientUnit unit) => unit.Value;
}
