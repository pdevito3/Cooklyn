namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record RecipeForUpdateDto
{
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public string? Rating { get; init; }
    public string? Source { get; init; }
    public bool IsFavorite { get; init; }
    public int? Servings { get; init; }
    public string? Steps { get; init; }
    public string? Notes { get; init; }
}
