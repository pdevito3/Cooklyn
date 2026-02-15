namespace Cooklyn.Server.Domain.ShoppingLists.DomainEvents;

public sealed record ShoppingListCreated(ShoppingList ShoppingList) : DomainEvent;
