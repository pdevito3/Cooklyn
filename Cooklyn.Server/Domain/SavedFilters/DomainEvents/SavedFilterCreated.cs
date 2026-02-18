namespace Cooklyn.Server.Domain.SavedFilters.DomainEvents;

public sealed record SavedFilterCreated(SavedFilter SavedFilter) : DomainEvent;
