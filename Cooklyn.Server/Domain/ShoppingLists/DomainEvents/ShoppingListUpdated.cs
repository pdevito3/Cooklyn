namespace Cooklyn.Server.Domain.ShoppingLists.DomainEvents;

public sealed record ShoppingListUpdated(string Id) : DomainEvent;
