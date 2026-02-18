namespace Cooklyn.Server.Domain.RecentSearches;

using Exceptions;
using RecentSearches.DomainEvents;
using RecentSearches.Models;

public class RecentSearch : BaseEntity, ITenantable
{
    public string TenantId { get; private set; } = default!;
    public string SearchType { get; private set; } = default!;
    public string SearchText { get; private set; } = default!;
    public string? ResourceType { get; private set; }
    public string? ResourceId { get; private set; }

    public static RecentSearch Create(RecentSearchForCreation forCreation)
    {
        var recentSearch = new RecentSearch
        {
            TenantId = forCreation.TenantId,
            SearchType = forCreation.SearchType,
            SearchText = forCreation.SearchText,
            ResourceType = forCreation.ResourceType,
            ResourceId = forCreation.ResourceId
        };

        Validate(recentSearch);
        recentSearch.QueueDomainEvent(new RecentSearchCreated(recentSearch));

        return recentSearch;
    }

    private static void Validate(RecentSearch recentSearch)
    {
        ValidationException.ThrowWhenNullOrWhitespace(recentSearch.TenantId, "Please provide a tenant.");
        ValidationException.ThrowWhenNullOrWhitespace(recentSearch.SearchType, "Please provide a search type.");
        ValidationException.ThrowWhenNullOrWhitespace(recentSearch.SearchText, "Please provide search text.");
        ValidationException.Must(
            recentSearch.SearchType is "query" or "selection",
            "Search type must be 'query' or 'selection'.");
    }

    protected RecentSearch() { } // EF Core
}
