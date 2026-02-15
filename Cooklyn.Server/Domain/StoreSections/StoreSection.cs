namespace Cooklyn.Server.Domain.StoreSections;

using Exceptions;
using StoreSections.DomainEvents;
using StoreSections.Models;

public class StoreSection : BaseEntity, ITenantable
{
    public string TenantId { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    public static StoreSection Create(StoreSectionForCreation forCreation)
    {
        var storeSection = new StoreSection
        {
            TenantId = forCreation.TenantId,
            Name = forCreation.Name
        };

        ValidateStoreSection(storeSection);
        storeSection.QueueDomainEvent(new StoreSectionCreated(storeSection));

        return storeSection;
    }

    public StoreSection Update(StoreSectionForUpdate forUpdate)
    {
        Name = forUpdate.Name;

        ValidateStoreSection(this);
        QueueDomainEvent(new StoreSectionUpdated(Id));

        return this;
    }

    private static void ValidateStoreSection(StoreSection storeSection)
    {
        ValidationException.ThrowWhenNullOrWhitespace(storeSection.TenantId, "Please provide a tenant.");
        ValidationException.ThrowWhenNullOrWhitespace(storeSection.Name, "Please provide a store section name.");
    }

    protected StoreSection() { } // EF Core
}
