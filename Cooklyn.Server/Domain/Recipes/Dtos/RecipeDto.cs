namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record RecipeDto
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    /// <summary>
    /// Pre-signed URL for accessing the recipe image. Generated on read.
    /// </summary>
    public string? ImageUrl { get; init; }
    /// <summary>
    /// S3 bucket where the image is stored.
    /// </summary>
    public string? ImageS3Bucket { get; init; }
    /// <summary>
    /// S3 key for the image file.
    /// </summary>
    public string? ImageS3Key { get; init; }
    public string Rating { get; init; } = default!;
    public string? Source { get; init; }
    public int? Servings { get; init; }
    public string? Steps { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public IReadOnlyList<string> Flags { get; init; } = [];
    public IReadOnlyList<IngredientDto> Ingredients { get; init; } = [];
    public NutritionInfoDto? NutritionInfo { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset? LastModifiedOn { get; init; }
}
