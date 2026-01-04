namespace Cooklyn.Server.Domain.Recipes.DomainEvents;

public sealed record RecipeUpdated(Guid Id) : DomainEvent;
