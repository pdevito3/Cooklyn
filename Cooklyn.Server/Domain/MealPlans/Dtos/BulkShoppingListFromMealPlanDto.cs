namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record BulkShoppingListFromMealPlanDto
{
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public string? ShoppingListId { get; init; }
    public string? NewShoppingListName { get; init; }
    public IReadOnlyList<string>? ExcludedEntryIds { get; init; }
    public IReadOnlyList<MealPlanEntryIngredientSelectionDto>? EntryIngredientSelections { get; init; }
}
