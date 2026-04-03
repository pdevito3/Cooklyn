namespace Cooklyn.Server.Domain;

using System.ComponentModel.DataAnnotations.Schema;

public abstract class BaseEntity
{

    public string Id { get; private set; } = default!;

    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset? LastModifiedOn { get; private set; }

    public bool IsDeleted { get; private set; }

    private readonly List<DomainEvent> _domainEvents = [];
    [NotMapped]
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void QueueDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void UpdateCreationProperties(DateTimeOffset createdOn)
    {
        CreatedOn = createdOn;
    }

    public void UpdateModifiedProperties(DateTimeOffset? lastModifiedOn)
    {
        LastModifiedOn = lastModifiedOn;
    }

    public void UpdateIsDeleted(bool isDeleted)
    {
        IsDeleted = isDeleted;
    }

    public void OverrideId(string id)
    {
        Id = id;
    }
}
