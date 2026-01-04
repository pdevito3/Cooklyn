namespace Cooklyn.Server.Domain.Tenants.DomainEvents;

public sealed record TenantUpdated(Guid Id) : DomainEvent;
