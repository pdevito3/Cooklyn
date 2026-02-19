namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanQueueDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public bool IsDefault { get; init; }
    public IReadOnlyList<MealPlanQueueItemDto> Items { get; init; } = [];
}
