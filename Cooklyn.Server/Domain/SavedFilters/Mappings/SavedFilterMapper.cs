namespace Cooklyn.Server.Domain.SavedFilters.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class SavedFilterMapper
{
    [MapperIgnoreSource(nameof(SavedFilter.CreatedBy))]
    [MapperIgnoreSource(nameof(SavedFilter.LastModifiedBy))]
    [MapperIgnoreSource(nameof(SavedFilter.CreatedOn))]
    [MapperIgnoreSource(nameof(SavedFilter.LastModifiedOn))]
    [MapperIgnoreSource(nameof(SavedFilter.IsDeleted))]
    [MapperIgnoreSource(nameof(SavedFilter.DomainEvents))]
    public static partial SavedFilterDto ToSavedFilterDto(this SavedFilter savedFilter);

    public static partial IQueryable<SavedFilterDto> ToSavedFilterDtoQueryable(this IQueryable<SavedFilter> queryable);

    public static SavedFilterForCreation ToSavedFilterForCreation(this SavedFilterForCreationDto dto, string tenantId)
    {
        return new SavedFilterForCreation
        {
            TenantId = tenantId,
            Name = dto.Name,
            Context = dto.Context,
            FilterStateJson = dto.FilterStateJson
        };
    }

    public static partial SavedFilterForUpdate ToSavedFilterForUpdate(this SavedFilterForUpdateDto dto);
}
