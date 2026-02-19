namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanDayDto
{
    public DateOnly Date { get; init; }
    public IReadOnlyList<MealPlanEntryDto> Entries { get; init; } = [];
}
