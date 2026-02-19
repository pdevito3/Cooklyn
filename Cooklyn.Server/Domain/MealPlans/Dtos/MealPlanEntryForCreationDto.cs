namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanEntryForCreationDto
{
    public DateOnly Date { get; init; }
    public string EntryType { get; init; } = default!;
    public string? RecipeId { get; init; }
    public string Title { get; init; } = default!;
    public decimal Scale { get; init; } = 1.0m;
    public int SortOrder { get; init; }
}
