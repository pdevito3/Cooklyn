namespace Cooklyn.Server.Domain.Ingredients.DomainEvents;

public sealed record IngredientCreated(Ingredient Ingredient) : DomainEvent;
