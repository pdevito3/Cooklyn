namespace Cooklyn.Server.Domain.Stores;

using Exceptions;
using Stores.DomainEvents;
using Stores.Models;

public class Store : BaseEntity, ITenantable
{
    public string TenantId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Address { get; private set; }

    private readonly List<StoreAisle> _storeAisles = [];
    public IReadOnlyCollection<StoreAisle> StoreAisles => _storeAisles.AsReadOnly();

    private readonly List<StoreDefaultCollection> _storeDefaultCollections = [];
    public IReadOnlyCollection<StoreDefaultCollection> StoreDefaultCollections => _storeDefaultCollections.AsReadOnly();

    public static Store Create(StoreForCreation forCreation)
    {
        var store = new Store
        {
            TenantId = forCreation.TenantId,
            Name = forCreation.Name,
            Address = forCreation.Address
        };

        ValidateStore(store);
        store.QueueDomainEvent(new StoreCreated(store));

        return store;
    }

    public Store Update(StoreForUpdate forUpdate)
    {
        Name = forUpdate.Name;
        Address = forUpdate.Address;

        ValidateStore(this);
        QueueDomainEvent(new StoreUpdated(Id));

        return this;
    }

    public Store SetAisles(List<StoreAisle> aisles)
    {
        _storeAisles.Clear();
        _storeAisles.AddRange(aisles);
        QueueDomainEvent(new StoreUpdated(Id));
        return this;
    }

    public Store SetDefaultCollections(List<StoreDefaultCollection> collections)
    {
        _storeDefaultCollections.Clear();
        _storeDefaultCollections.AddRange(collections);
        QueueDomainEvent(new StoreUpdated(Id));
        return this;
    }

    private static void ValidateStore(Store store)
    {
        ValidationException.ThrowWhenNullOrWhitespace(store.TenantId, "Please provide a tenant.");
        ValidationException.ThrowWhenNullOrWhitespace(store.Name, "Please provide a store name.");
    }

    protected Store() { } // EF Core
}
