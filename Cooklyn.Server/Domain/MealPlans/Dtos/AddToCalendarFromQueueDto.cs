namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record AddToCalendarFromQueueDto
{
    public string QueueItemId { get; init; } = default!;
    public DateOnly TargetDate { get; init; }
    public int SortOrder { get; init; }
}
