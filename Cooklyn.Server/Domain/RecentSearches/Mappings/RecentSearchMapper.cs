namespace Cooklyn.Server.Domain.RecentSearches.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class RecentSearchMapper
{
    [MapperIgnoreSource(nameof(RecentSearch.CreatedBy))]
    [MapperIgnoreSource(nameof(RecentSearch.LastModifiedBy))]
    [MapperIgnoreSource(nameof(RecentSearch.LastModifiedOn))]
    [MapperIgnoreSource(nameof(RecentSearch.IsDeleted))]
    [MapperIgnoreSource(nameof(RecentSearch.DomainEvents))]
    [MapperIgnoreSource(nameof(RecentSearch.TenantId))]
    public static partial RecentSearchDto ToRecentSearchDto(this RecentSearch recentSearch);

    public static RecentSearchForCreation ToRecentSearchForCreation(this RecentSearchForCreationDto dto, string tenantId)
    {
        return new RecentSearchForCreation
        {
            TenantId = tenantId,
            SearchType = dto.SearchType,
            SearchText = dto.SearchText,
            ResourceType = dto.ResourceType,
            ResourceId = dto.ResourceId
        };
    }
}
