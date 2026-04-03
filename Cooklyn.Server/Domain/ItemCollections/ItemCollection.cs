namespace Cooklyn.Server.Domain.ItemCollections;

using Exceptions;
using ItemCollections.DomainEvents;
using ItemCollections.Models;

public class ItemCollection : BaseEntity
{
    public string Name { get; private set; } = default!;

    private readonly List<ItemCollectionItem> _items = [];
    public IReadOnlyCollection<ItemCollectionItem> Items => _items.AsReadOnly();

    public static ItemCollection Create(ItemCollectionForCreation forCreation)
    {
        var collection = new ItemCollection
        {
            Name = forCreation.Name
        };

        ValidateItemCollection(collection);
        collection.QueueDomainEvent(new ItemCollectionCreated(collection));

        return collection;
    }

    public ItemCollection Update(ItemCollectionForUpdate forUpdate)
    {
        Name = forUpdate.Name;

        ValidateItemCollection(this);
        QueueDomainEvent(new ItemCollectionUpdated(Id));

        return this;
    }

    private static void ValidateItemCollection(ItemCollection collection)
    {
        ValidationException.ThrowWhenNullOrWhitespace(collection.Name, "Please provide a collection name.");
    }

    protected ItemCollection() { } // EF Core
}
