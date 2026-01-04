namespace Cooklyn.Server.Domain.Tags.DomainEvents;

public sealed record TagUpdated(Guid Id) : DomainEvent;
