namespace Cooklyn.Server.Domain.Ingredients.DomainEvents;

public sealed record IngredientUpdated(Guid Id) : DomainEvent;
