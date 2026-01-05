namespace Cooklyn.Server.Domain.Ingredients;

using Exceptions;
using Ingredients.DomainEvents;
using Ingredients.Models;

public class Ingredient : BaseEntity
{
    public Guid RecipeId { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal? Quantity { get; private set; }
    public UnitOfMeasure Unit { get; private set; } = default!;
    public int SortOrder { get; private set; }
    public string? Notes { get; private set; }

    public static Ingredient Create(IngredientForCreation forCreation)
    {
        var ingredient = new Ingredient
        {
            RecipeId = forCreation.RecipeId,
            Name = forCreation.Name,
            Quantity = forCreation.Quantity,
            Unit = UnitOfMeasure.Of(forCreation.Unit),
            SortOrder = forCreation.SortOrder,
            Notes = forCreation.Notes
        };

        ValidateIngredient(ingredient);
        ingredient.QueueDomainEvent(new IngredientCreated(ingredient));

        return ingredient;
    }

    public Ingredient Update(IngredientForUpdate forUpdate)
    {
        Name = forUpdate.Name;
        Quantity = forUpdate.Quantity;
        Unit = UnitOfMeasure.Of(forUpdate.Unit);
        SortOrder = forUpdate.SortOrder;
        Notes = forUpdate.Notes;

        ValidateIngredient(this);
        QueueDomainEvent(new IngredientUpdated(Id));

        return this;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    public string GetDisplayText()
    {
        if (Quantity.HasValue && !Unit.IsEmpty)
        {
            return $"{Quantity} {Unit.Value} {Name}";
        }
        else if (Quantity.HasValue)
        {
            return $"{Quantity} {Name}";
        }
        else if (!Unit.IsEmpty)
        {
            return $"{Unit.Value} {Name}";
        }
        else
        {
            return Name;
        }
    }

    private static void ValidateIngredient(Ingredient ingredient)
    {
        ValidationException.ThrowWhenEmpty(ingredient.RecipeId, "Please provide a recipe.");
        ValidationException.ThrowWhenNullOrWhitespace(ingredient.Name, "Please provide an ingredient name.");
    }

    protected Ingredient() { } // EF Core
}
