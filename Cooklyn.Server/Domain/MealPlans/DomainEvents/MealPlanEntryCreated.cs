namespace Cooklyn.Server.Domain.MealPlans.DomainEvents;

public sealed record MealPlanEntryCreated(MealPlanEntry MealPlanEntry) : DomainEvent;
