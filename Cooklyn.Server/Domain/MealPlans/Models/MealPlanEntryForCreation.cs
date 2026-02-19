namespace Cooklyn.Server.Domain.MealPlans.Models;

public sealed record MealPlanEntryForCreation
{
    public string TenantId { get; init; } = default!;
    public DateOnly Date { get; init; }
    public string EntryType { get; init; } = default!;
    public string? RecipeId { get; init; }
    public string Title { get; init; } = default!;
    public decimal Scale { get; init; } = 1.0m;
    public int SortOrder { get; init; }
}
