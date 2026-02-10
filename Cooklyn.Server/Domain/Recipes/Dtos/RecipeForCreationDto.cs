namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record RecipeForCreationDto
{
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    /// <summary>
    /// S3 bucket where the image is stored. Set after uploading via the image upload endpoint.
    /// </summary>
    public string? ImageS3Bucket { get; init; }
    /// <summary>
    /// S3 key for the image file. Set after uploading via the image upload endpoint.
    /// </summary>
    public string? ImageS3Key { get; init; }
    public string? Rating { get; init; }
    public string? Source { get; init; }
    public bool IsFavorite { get; init; }
    public int? Servings { get; init; }
    public string? Steps { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<string> TagIds { get; init; } = [];
    public IReadOnlyList<string> Flags { get; init; } = [];
    public IReadOnlyList<IngredientForCreationDto> Ingredients { get; init; } = [];
    public NutritionInfoForCreationDto? NutritionInfo { get; init; }
}
