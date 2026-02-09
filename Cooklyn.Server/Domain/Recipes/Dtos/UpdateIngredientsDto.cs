namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record UpdateIngredientsDto
{
    public IReadOnlyList<IngredientForCreationDto> Ingredients { get; init; } = [];
}
