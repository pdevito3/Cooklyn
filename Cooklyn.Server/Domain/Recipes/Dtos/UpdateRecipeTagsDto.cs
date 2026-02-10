namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record UpdateRecipeTagsDto
{
    public IReadOnlyList<string> TagIds { get; init; } = [];
}
