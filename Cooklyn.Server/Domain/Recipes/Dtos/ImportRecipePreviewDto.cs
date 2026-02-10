namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record ImportRecipePreviewRequestDto
{
    public string Url { get; init; } = default!;
}

public sealed record ImportRecipePreviewDto
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Source { get; init; }
    public int? Servings { get; init; }
    public string? Steps { get; init; }
    public IReadOnlyList<IngredientForCreationDto> Ingredients { get; init; } = [];
    public IReadOnlyList<ImportImageDto> Images { get; init; } = [];
}

public sealed record ImportImageDto
{
    public string Url { get; init; } = default!;
    public string? Alt { get; init; }
}

public sealed record UploadImageFromUrlRequestDto
{
    public string ImageUrl { get; init; } = default!;
}
