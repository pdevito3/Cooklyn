namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanEntryIngredientSelectionDto
{
    public string EntryId { get; init; } = default!;
    public IReadOnlyList<string>? IngredientIds { get; init; }
}
