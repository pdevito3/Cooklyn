namespace Cooklyn.Server.Domain.ShoppingLists;

using Recipes;
using Models;

public class ShoppingListItem : BaseEntity
{
    public string ShoppingListId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public decimal? Quantity { get; private set; }
    public IngredientUnit Unit { get; private set; } = new(string.Empty);
    public string? StoreSectionId { get; private set; }
    public bool IsChecked { get; private set; }
    public DateTimeOffset? CheckedOn { get; private set; }
    public string? Notes { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<ShoppingListItemRecipeSource> _recipeSources = [];
    public IReadOnlyCollection<ShoppingListItemRecipeSource> RecipeSources => _recipeSources.AsReadOnly();

    public static ShoppingListItem Create(ShoppingListItemForCreation forCreation)
    {
        return new ShoppingListItem
        {
            ShoppingListId = forCreation.ShoppingListId,
            Name = forCreation.Name,
            Quantity = forCreation.Quantity,
            Unit = new IngredientUnit(forCreation.Unit ?? string.Empty),
            StoreSectionId = forCreation.StoreSectionId,
            Notes = forCreation.Notes,
            SortOrder = forCreation.SortOrder,
            IsChecked = false
        };
    }

    public ShoppingListItem Update(ShoppingListItemForUpdate forUpdate)
    {
        Name = forUpdate.Name;
        Quantity = forUpdate.Quantity;
        Unit = new IngredientUnit(forUpdate.Unit ?? string.Empty);
        StoreSectionId = forUpdate.StoreSectionId;
        Notes = forUpdate.Notes;
        SortOrder = forUpdate.SortOrder;
        return this;
    }

    public ShoppingListItem ToggleCheck()
    {
        IsChecked = !IsChecked;
        CheckedOn = IsChecked ? DateTimeOffset.UtcNow : null;
        return this;
    }

    public ShoppingListItem MergeQuantity(decimal amount, IngredientUnit? unit)
    {
        if (Quantity.HasValue && unit != null && Unit.Value == unit.Value)
        {
            Quantity += amount;
        }
        return this;
    }

    public ShoppingListItem AddRecipeSource(ShoppingListItemRecipeSource source)
    {
        _recipeSources.Add(source);
        return this;
    }

    protected ShoppingListItem() { } // EF Core
}
