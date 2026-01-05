namespace Cooklyn.Server.Domain.Ingredients.Dtos;

public sealed record ReorderIngredientsDto
{
    public IReadOnlyList<Guid> IngredientIds { get; init; } = [];
}
