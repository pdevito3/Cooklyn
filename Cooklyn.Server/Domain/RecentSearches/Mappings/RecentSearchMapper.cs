namespace Cooklyn.Server.Domain.RecentSearches.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class RecentSearchMapper
{
    [MapperIgnoreSource(nameof(RecentSearch.LastModifiedOn))]
    [MapperIgnoreSource(nameof(RecentSearch.IsDeleted))]
    [MapperIgnoreSource(nameof(RecentSearch.DomainEvents))]
    public static partial RecentSearchDto ToRecentSearchDto(this RecentSearch recentSearch);

    public static RecentSearchForCreation ToRecentSearchForCreation(this RecentSearchForCreationDto dto)
    {
        return new RecentSearchForCreation
        {
            SearchType = dto.SearchType,
            SearchText = dto.SearchText,
            ResourceType = dto.ResourceType,
            ResourceId = dto.ResourceId
        };
    }
}
