namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record UpdateRecipeTagsDto
{
    public IReadOnlyList<Guid> TagIds { get; init; } = [];
}
