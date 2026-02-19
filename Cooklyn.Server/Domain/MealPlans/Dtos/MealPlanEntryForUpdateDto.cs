namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanEntryForUpdateDto
{
    public string Title { get; init; } = default!;
    public decimal Scale { get; init; } = 1.0m;
    public int SortOrder { get; init; }
}
