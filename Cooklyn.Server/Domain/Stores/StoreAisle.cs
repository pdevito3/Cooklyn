namespace Cooklyn.Server.Domain.Stores;

using StoreSections;

public class StoreAisle : BaseEntity
{
    public string StoreId { get; private set; } = default!;
    public string StoreSectionId { get; private set; } = default!;
    public int SortOrder { get; private set; }
    public string? CustomName { get; private set; }

    public static StoreAisle Create(string storeId, string storeSectionId, int sortOrder, string? customName)
    {
        return new StoreAisle
        {
            StoreId = storeId,
            StoreSectionId = storeSectionId,
            SortOrder = sortOrder,
            CustomName = customName
        };
    }

    public StoreAisle Update(int sortOrder, string? customName)
    {
        SortOrder = sortOrder;
        CustomName = customName;
        return this;
    }

    protected StoreAisle() { } // EF Core
}
