namespace Cooklyn.Server.Domain.SavedFilters.DomainEvents;

public sealed record SavedFilterUpdated(string Id) : DomainEvent;
