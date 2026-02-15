namespace Cooklyn.Server.Domain.ItemCollections;

using Recipes;

public class ItemCollectionItem : BaseEntity
{
    public string ItemCollectionId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public decimal? Quantity { get; private set; }
    public IngredientUnit Unit { get; private set; } = new(string.Empty);
    public string? StoreSectionId { get; private set; }
    public int SortOrder { get; private set; }

    public static ItemCollectionItem Create(string itemCollectionId, string name, decimal? quantity, string? unit, string? storeSectionId, int sortOrder)
    {
        return new ItemCollectionItem
        {
            ItemCollectionId = itemCollectionId,
            Name = name,
            Quantity = quantity,
            Unit = new IngredientUnit(unit ?? string.Empty),
            StoreSectionId = storeSectionId,
            SortOrder = sortOrder
        };
    }

    public ItemCollectionItem Update(string name, decimal? quantity, string? unit, string? storeSectionId, int sortOrder)
    {
        Name = name;
        Quantity = quantity;
        Unit = new IngredientUnit(unit ?? string.Empty);
        StoreSectionId = storeSectionId;
        SortOrder = sortOrder;
        return this;
    }

    protected ItemCollectionItem() { } // EF Core
}
