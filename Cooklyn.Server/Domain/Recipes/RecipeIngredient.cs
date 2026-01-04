namespace Cooklyn.Server.Domain.Recipes;

using Exceptions;
using Recipes.Models;

public class RecipeIngredient : BaseEntity
{
    public Guid RecipeId { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal? Quantity { get; private set; }
    public UnitOfMeasure Unit { get; private set; } = default!;
    public int SortOrder { get; private set; }
    public string? Notes { get; private set; }

    public static RecipeIngredient Create(RecipeIngredientForCreation forCreation)
    {
        var ingredient = new RecipeIngredient
        {
            RecipeId = forCreation.RecipeId,
            Name = forCreation.Name,
            Quantity = forCreation.Quantity,
            Unit = UnitOfMeasure.Of(forCreation.Unit),
            SortOrder = forCreation.SortOrder,
            Notes = forCreation.Notes
        };

        ValidateIngredient(ingredient);

        return ingredient;
    }

    public RecipeIngredient Update(RecipeIngredientForUpdate forUpdate)
    {
        Name = forUpdate.Name;
        Quantity = forUpdate.Quantity;
        Unit = UnitOfMeasure.Of(forUpdate.Unit);
        SortOrder = forUpdate.SortOrder;
        Notes = forUpdate.Notes;

        ValidateIngredient(this);

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

    private static void ValidateIngredient(RecipeIngredient ingredient)
    {
        ValidationException.ThrowWhenEmpty(ingredient.RecipeId, "Please provide a recipe.");
        ValidationException.ThrowWhenNullOrWhitespace(ingredient.Name, "Please provide an ingredient name.");
    }

    protected RecipeIngredient() { } // EF Core
}
