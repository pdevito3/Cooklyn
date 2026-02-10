namespace Cooklyn.Server.Domain.Tags;

using Exceptions;
using Tags.DomainEvents;
using Tags.Models;

public class Tag : BaseEntity, ITenantable
{
    public string TenantId { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    public static Tag Create(TagForCreation tagForCreation)
    {
        var tag = new Tag
        {
            TenantId = tagForCreation.TenantId,
            Name = tagForCreation.Name
        };

        ValidateTag(tag);
        tag.QueueDomainEvent(new TagCreated(tag));

        return tag;
    }

    public Tag Update(TagForUpdate tagForUpdate)
    {
        Name = tagForUpdate.Name;

        ValidateTag(this);
        QueueDomainEvent(new TagUpdated(Id));

        return this;
    }

    private static void ValidateTag(Tag tag)
    {
        ValidationException.ThrowWhenNullOrWhitespace(tag.TenantId, "Please provide a tenant.");
        ValidationException.ThrowWhenNullOrWhitespace(tag.Name, "Please provide a tag name.");
    }

    protected Tag() { } // EF Core
}
