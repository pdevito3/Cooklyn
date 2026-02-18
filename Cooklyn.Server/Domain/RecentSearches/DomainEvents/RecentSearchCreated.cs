namespace Cooklyn.Server.Domain.RecentSearches.DomainEvents;

public sealed record RecentSearchCreated(RecentSearch RecentSearch) : DomainEvent;
