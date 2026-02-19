namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanQueueItemDto
{
    public string Id { get; init; } = default!;
    public string? RecipeId { get; init; }
    public string Title { get; init; } = default!;
    public decimal Scale { get; init; }
    public int SortOrder { get; init; }
    public string? ImageUrl { get; init; }
}
