namespace Cooklyn.Server.Domain.MealPlans;

using MealPlans.Models;

public class MealPlanQueueItem : BaseEntity
{
    public string QueueId { get; private set; } = default!;
    public string? RecipeId { get; private set; }
    public string Title { get; private set; } = default!;
    public decimal Scale { get; private set; } = 1.0m;
    public int SortOrder { get; private set; }

    public static MealPlanQueueItem Create(MealPlanQueueItemForCreation forCreation)
    {
        return new MealPlanQueueItem
        {
            QueueId = forCreation.QueueId,
            RecipeId = forCreation.RecipeId,
            Title = forCreation.Title,
            Scale = forCreation.Scale,
            SortOrder = forCreation.SortOrder
        };
    }

    protected MealPlanQueueItem() { } // EF Core
}
