namespace Cooklyn.Server.Domain.MealPlans;

using Exceptions;
using MealPlans.DomainEvents;
using MealPlans.Models;

public class MealPlanEntry : BaseEntity, ITenantable
{
    public string TenantId { get; private set; } = default!;
    public DateOnly Date { get; private set; }
    public MealPlanEntryType EntryType { get; private set; } = MealPlanEntryType.Recipe();
    public string? RecipeId { get; private set; }
    public string Title { get; private set; } = default!;
    public decimal Scale { get; private set; } = 1.0m;
    public int SortOrder { get; private set; }

    public static MealPlanEntry Create(MealPlanEntryForCreation forCreation)
    {
        var entry = new MealPlanEntry
        {
            TenantId = forCreation.TenantId,
            Date = forCreation.Date,
            EntryType = MealPlanEntryType.Of(forCreation.EntryType),
            RecipeId = forCreation.RecipeId,
            Title = forCreation.Title,
            Scale = forCreation.Scale,
            SortOrder = forCreation.SortOrder
        };

        ValidateEntry(entry);
        entry.QueueDomainEvent(new MealPlanEntryCreated(entry));

        return entry;
    }

    public MealPlanEntry Update(MealPlanEntryForUpdate forUpdate)
    {
        Title = forUpdate.Title;
        Scale = forUpdate.Scale;
        SortOrder = forUpdate.SortOrder;

        ValidateEntry(this);
        QueueDomainEvent(new MealPlanEntryUpdated(Id));

        return this;
    }

    public MealPlanEntry MoveTo(DateOnly newDate, int sortOrder)
    {
        Date = newDate;
        SortOrder = sortOrder;
        return this;
    }

    public MealPlanEntry Copy(string tenantId, DateOnly targetDate, int sortOrder)
    {
        return new MealPlanEntry
        {
            TenantId = tenantId,
            Date = targetDate,
            EntryType = new MealPlanEntryType(EntryType.Value),
            RecipeId = RecipeId,
            Title = Title,
            Scale = Scale,
            SortOrder = sortOrder
        };
    }

    private static void ValidateEntry(MealPlanEntry entry)
    {
        ValidationException.ThrowWhenNullOrWhitespace(entry.TenantId, "Please provide a tenant.");
        ValidationException.ThrowWhenNullOrWhitespace(entry.Title, "Please provide a title.");
    }

    protected MealPlanEntry() { } // EF Core
}
