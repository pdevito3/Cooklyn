namespace Cooklyn.Server.Domain.Tags.DomainEvents;

public sealed record TagCreated(Tag Tag) : DomainEvent;
