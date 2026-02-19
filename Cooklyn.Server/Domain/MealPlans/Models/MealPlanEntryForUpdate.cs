namespace Cooklyn.Server.Domain.MealPlans.Models;

public sealed record MealPlanEntryForUpdate
{
    public string Title { get; init; } = default!;
    public decimal Scale { get; init; } = 1.0m;
    public int SortOrder { get; init; }
}
