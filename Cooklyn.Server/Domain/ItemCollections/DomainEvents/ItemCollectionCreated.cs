namespace Cooklyn.Server.Domain.ItemCollections.DomainEvents;

public sealed record ItemCollectionCreated(ItemCollection ItemCollection) : DomainEvent;
