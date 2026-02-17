namespace Cooklyn.Server.Domain.Stores.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class StoreMapper
{
    [MapperIgnoreSource(nameof(Store.CreatedBy))]
    [MapperIgnoreSource(nameof(Store.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Store.CreatedOn))]
    [MapperIgnoreSource(nameof(Store.LastModifiedOn))]
    [MapperIgnoreSource(nameof(Store.IsDeleted))]
    [MapperIgnoreSource(nameof(Store.DomainEvents))]
    public static partial StoreDto ToStoreDto(this Store store);

    [MapperIgnoreSource(nameof(StoreAisle.CreatedBy))]
    [MapperIgnoreSource(nameof(StoreAisle.LastModifiedBy))]
    [MapperIgnoreSource(nameof(StoreAisle.CreatedOn))]
    [MapperIgnoreSource(nameof(StoreAisle.LastModifiedOn))]
    [MapperIgnoreSource(nameof(StoreAisle.IsDeleted))]
    [MapperIgnoreSource(nameof(StoreAisle.DomainEvents))]
    public static partial StoreAisleDto ToStoreAisleDto(this StoreAisle storeAisle);

    [MapperIgnoreSource(nameof(StoreDefaultCollection.CreatedBy))]
    [MapperIgnoreSource(nameof(StoreDefaultCollection.LastModifiedBy))]
    [MapperIgnoreSource(nameof(StoreDefaultCollection.CreatedOn))]
    [MapperIgnoreSource(nameof(StoreDefaultCollection.LastModifiedOn))]
    [MapperIgnoreSource(nameof(StoreDefaultCollection.IsDeleted))]
    [MapperIgnoreSource(nameof(StoreDefaultCollection.DomainEvents))]
    [MapperIgnoreSource(nameof(StoreDefaultCollection.StoreId))]
    [MapProperty(nameof(StoreDefaultCollection.ItemCollection) + "." + nameof(ItemCollections.ItemCollection.Name), nameof(StoreDefaultCollectionDto.ItemCollectionName))]
    public static partial StoreDefaultCollectionDto ToStoreDefaultCollectionDto(this StoreDefaultCollection storeDefaultCollection);

    public static StoreForCreation ToStoreForCreation(this StoreForCreationDto dto, string tenantId)
    {
        return new StoreForCreation
        {
            TenantId = tenantId,
            Name = dto.Name,
            Address = dto.Address
        };
    }

    public static partial StoreForUpdate ToStoreForUpdate(this StoreForUpdateDto dto);
}
