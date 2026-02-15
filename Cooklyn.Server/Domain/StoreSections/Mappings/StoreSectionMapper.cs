namespace Cooklyn.Server.Domain.StoreSections.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class StoreSectionMapper
{
    [MapperIgnoreSource(nameof(StoreSection.CreatedBy))]
    [MapperIgnoreSource(nameof(StoreSection.LastModifiedBy))]
    [MapperIgnoreSource(nameof(StoreSection.CreatedOn))]
    [MapperIgnoreSource(nameof(StoreSection.LastModifiedOn))]
    [MapperIgnoreSource(nameof(StoreSection.IsDeleted))]
    [MapperIgnoreSource(nameof(StoreSection.DomainEvents))]
    public static partial StoreSectionDto ToStoreSectionDto(this StoreSection storeSection);

    public static partial IQueryable<StoreSectionDto> ToStoreSectionDtoQueryable(this IQueryable<StoreSection> queryable);

    public static StoreSectionForCreation ToStoreSectionForCreation(this StoreSectionForCreationDto dto, string tenantId)
    {
        return new StoreSectionForCreation
        {
            TenantId = tenantId,
            Name = dto.Name
        };
    }

    public static partial StoreSectionForUpdate ToStoreSectionForUpdate(this StoreSectionForUpdateDto dto);
}
