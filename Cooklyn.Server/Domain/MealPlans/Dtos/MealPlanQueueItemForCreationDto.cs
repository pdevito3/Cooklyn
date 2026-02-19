namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanQueueItemForCreationDto
{
    public string? RecipeId { get; init; }
    public string Title { get; init; } = default!;
    public decimal Scale { get; init; } = 1.0m;
}
