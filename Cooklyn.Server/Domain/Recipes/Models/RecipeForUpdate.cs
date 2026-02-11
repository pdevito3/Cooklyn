namespace Cooklyn.Server.Domain.Recipes.Models;

public sealed record RecipeForUpdate
{
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public string? ImageS3Bucket { get; init; }
    public string? ImageS3Key { get; init; }
    public string? Rating { get; init; }
    public string? Source { get; init; }
    public int? Servings { get; init; }
    public string? Steps { get; init; }
    public string? Notes { get; init; }
}
