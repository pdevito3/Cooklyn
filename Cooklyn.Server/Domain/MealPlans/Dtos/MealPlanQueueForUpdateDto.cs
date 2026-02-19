namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanQueueForUpdateDto
{
    public string Name { get; init; } = default!;
}
