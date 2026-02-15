namespace Cooklyn.Server.Domain.Stores.DomainEvents;

public sealed record StoreCreated(Store Store) : DomainEvent;
