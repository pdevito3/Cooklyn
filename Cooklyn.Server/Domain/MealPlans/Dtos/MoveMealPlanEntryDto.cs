namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MoveMealPlanEntryDto
{
    public DateOnly TargetDate { get; init; }
    public int SortOrder { get; init; }
}
