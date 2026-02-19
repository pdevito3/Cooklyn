namespace Cooklyn.Server.Domain.MealPlans.Models;

public sealed record MealPlanQueueForUpdate
{
    public string Name { get; init; } = default!;
}
