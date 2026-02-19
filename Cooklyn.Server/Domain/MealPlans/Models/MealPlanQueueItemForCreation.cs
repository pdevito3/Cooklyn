namespace Cooklyn.Server.Domain.MealPlans.Models;

public sealed record MealPlanQueueItemForCreation
{
    public string QueueId { get; init; } = default!;
    public string? RecipeId { get; init; }
    public string Title { get; init; } = default!;
    public decimal Scale { get; init; } = 1.0m;
    public int SortOrder { get; init; }
}
