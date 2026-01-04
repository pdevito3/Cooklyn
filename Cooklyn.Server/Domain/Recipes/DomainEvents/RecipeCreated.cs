namespace Cooklyn.Server.Domain.Recipes.DomainEvents;

public sealed record RecipeCreated(Recipe Recipe) : DomainEvent;
