namespace Cooklyn.Server.Domain.MealPlans;

using Exceptions;
using MealPlans.Models;

public class MealPlanQueue : BaseEntity
{
    public string Name { get; private set; } = default!;
    public bool IsDefault { get; private set; }

    private readonly List<MealPlanQueueItem> _items = [];
    public IReadOnlyCollection<MealPlanQueueItem> Items => _items.AsReadOnly();

    public static MealPlanQueue Create(MealPlanQueueForCreation forCreation)
    {
        var queue = new MealPlanQueue
        {
            Name = forCreation.Name,
            IsDefault = forCreation.IsDefault
        };

        ValidateQueue(queue);
        return queue;
    }

    public static MealPlanQueue CreateDefault()
    {
        return new MealPlanQueue
        {
            Name = "General",
            IsDefault = true
        };
    }

    public MealPlanQueue Update(MealPlanQueueForUpdate forUpdate)
    {
        Name = forUpdate.Name;
        ValidateQueue(this);
        return this;
    }

    public MealPlanQueue AddItem(MealPlanQueueItem item)
    {
        _items.Add(item);
        return this;
    }

    public MealPlanQueue RemoveItem(MealPlanQueueItem item)
    {
        _items.Remove(item);
        return this;
    }

    private static void ValidateQueue(MealPlanQueue queue)
    {
        ValidationException.ThrowWhenNullOrWhitespace(queue.Name, "Please provide a queue name.");
    }

    protected MealPlanQueue() { } // EF Core
}
