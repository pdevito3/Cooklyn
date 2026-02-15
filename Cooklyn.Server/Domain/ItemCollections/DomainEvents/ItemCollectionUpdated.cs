namespace Cooklyn.Server.Domain.ItemCollections.DomainEvents;

public sealed record ItemCollectionUpdated(string Id) : DomainEvent;
