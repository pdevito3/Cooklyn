namespace Cooklyn.Server.Domain.SavedFilters;

using Exceptions;
using SavedFilters.DomainEvents;
using SavedFilters.Models;

public class SavedFilter : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Context { get; private set; } = default!;
    public string FilterStateJson { get; private set; } = default!;

    public static SavedFilter Create(SavedFilterForCreation forCreation)
    {
        var savedFilter = new SavedFilter
        {
            Name = forCreation.Name,
            Context = forCreation.Context,
            FilterStateJson = forCreation.FilterStateJson
        };

        Validate(savedFilter);
        savedFilter.QueueDomainEvent(new SavedFilterCreated(savedFilter));

        return savedFilter;
    }

    public SavedFilter Update(SavedFilterForUpdate forUpdate)
    {
        Name = forUpdate.Name;
        FilterStateJson = forUpdate.FilterStateJson;

        Validate(this);
        QueueDomainEvent(new SavedFilterUpdated(Id));

        return this;
    }

    private static void Validate(SavedFilter savedFilter)
    {
        ValidationException.ThrowWhenNullOrWhitespace(savedFilter.Name, "Please provide a filter name.");
        ValidationException.ThrowWhenNullOrWhitespace(savedFilter.Context, "Please provide a context.");
        ValidationException.ThrowWhenNullOrWhitespace(savedFilter.FilterStateJson, "Please provide a filter state.");
    }

    protected SavedFilter() { } // EF Core
}
