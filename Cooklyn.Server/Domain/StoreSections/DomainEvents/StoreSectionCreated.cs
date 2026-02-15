namespace Cooklyn.Server.Domain.StoreSections.DomainEvents;

public sealed record StoreSectionCreated(StoreSection StoreSection) : DomainEvent;
