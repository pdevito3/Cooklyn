namespace Cooklyn.Server.Domain.Stores;

using ItemCollections;

public class StoreDefaultCollection : BaseEntity
{
    public string StoreId { get; private set; } = default!;
    public string ItemCollectionId { get; private set; } = default!;

    public ItemCollection ItemCollection { get; private set; } = default!;

    public static StoreDefaultCollection Create(string storeId, string itemCollectionId)
    {
        return new StoreDefaultCollection
        {
            StoreId = storeId,
            ItemCollectionId = itemCollectionId
        };
    }

    protected StoreDefaultCollection() { } // EF Core
}
