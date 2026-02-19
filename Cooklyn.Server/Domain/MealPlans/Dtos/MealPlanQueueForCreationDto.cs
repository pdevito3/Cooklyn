namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanQueueForCreationDto
{
    public string Name { get; init; } = default!;
}
