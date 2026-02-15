namespace Cooklyn.Server.Domain.Stores.DomainEvents;

public sealed record StoreUpdated(string Id) : DomainEvent;
