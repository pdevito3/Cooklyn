namespace Cooklyn.Server.Domain.ShoppingLists;

using Exceptions;
using ShoppingLists.DomainEvents;
using ShoppingLists.Models;

public class ShoppingList : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? StoreId { get; private set; }
    public ShoppingListStatus Status { get; private set; } = ShoppingListStatus.Active();
    public DateTimeOffset? CompletedOn { get; private set; }

    private readonly List<ShoppingListItem> _items = [];
    public IReadOnlyCollection<ShoppingListItem> Items => _items.AsReadOnly();

    public static ShoppingList Create(ShoppingListForCreation forCreation)
    {
        var list = new ShoppingList
        {
            Name = forCreation.Name,
            StoreId = forCreation.StoreId,
            Status = ShoppingListStatus.Active()
        };

        ValidateShoppingList(list);
        list.QueueDomainEvent(new ShoppingListCreated(list));

        return list;
    }

    public ShoppingList Update(ShoppingListForUpdate forUpdate)
    {
        Name = forUpdate.Name;
        StoreId = forUpdate.StoreId;

        ValidateShoppingList(this);
        QueueDomainEvent(new ShoppingListUpdated(Id));

        return this;
    }

    public ShoppingList Complete()
    {
        Status = ShoppingListStatus.Completed();
        CompletedOn = DateTimeOffset.UtcNow;
        QueueDomainEvent(new ShoppingListUpdated(Id));
        return this;
    }

    public ShoppingList Reopen()
    {
        Status = ShoppingListStatus.Active();
        CompletedOn = null;
        QueueDomainEvent(new ShoppingListUpdated(Id));
        return this;
    }

    public ShoppingList AddItem(ShoppingListItem item)
    {
        _items.Add(item);
        return this;
    }

    public ShoppingList RemoveItem(ShoppingListItem item)
    {
        _items.Remove(item);
        return this;
    }

    private static void ValidateShoppingList(ShoppingList list)
    {
        ValidationException.ThrowWhenNullOrWhitespace(list.Name, "Please provide a shopping list name.");
    }

    protected ShoppingList() { } // EF Core
}
