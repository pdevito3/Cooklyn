namespace Cooklyn.Server.Domain.MealPlans.Models;

public sealed record MealPlanQueueForCreation
{
    public string TenantId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public bool IsDefault { get; init; }
}
