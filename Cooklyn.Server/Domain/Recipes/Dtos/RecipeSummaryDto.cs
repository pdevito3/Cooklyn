namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record RecipeSummaryDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    /// <summary>
    /// Pre-signed URL for accessing the recipe image. Generated on read.
    /// </summary>
    public string? ImageUrl { get; init; }
    public string Rating { get; init; } = default!;
    public bool IsFavorite { get; init; }
    public int? Servings { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public IReadOnlyList<string> Flags { get; init; } = [];
    public DateTimeOffset CreatedOn { get; init; }
}
