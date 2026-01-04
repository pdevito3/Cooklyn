namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record UpdateRecipeRatingDto
{
    public string Rating { get; init; } = default!;
}
