namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record RecipeSummaryDto
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    /// <summary>
    /// Pre-signed URL for accessing the recipe image. Generated on read.
    /// </summary>
    public string? ImageUrl { get; init; }
    public string Rating { get; init; } = default!;
    public string? Source { get; init; }
    public int? Servings { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public IReadOnlyList<string> Flags { get; init; } = [];
    public int IngredientCount { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
}
