namespace Cooklyn.Server.Domain.Tags.DomainEvents;

public sealed record TagUpdated(string Id) : DomainEvent;
