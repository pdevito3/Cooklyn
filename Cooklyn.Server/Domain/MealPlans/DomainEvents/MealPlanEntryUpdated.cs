namespace Cooklyn.Server.Domain.MealPlans.DomainEvents;

public sealed record MealPlanEntryUpdated(string Id) : DomainEvent;
