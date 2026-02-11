namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record CmtImportPreviewItemDto
{
    public int Index { get; init; }
    public string Title { get; init; } = default!;
    public string? Source { get; init; }
    public int? Servings { get; init; }
    public int IngredientCount { get; init; }
    public int? Rating { get; init; }
    public bool HasImage { get; init; }
    public bool IsDuplicate { get; init; }
    public List<string> Tags { get; init; } = [];
}

public sealed record CmtImportPreviewDto
{
    public IReadOnlyList<CmtImportPreviewItemDto> Recipes { get; init; } = [];
    public int TotalCount { get; init; }
    public int DuplicateCount { get; init; }
}

public sealed record CmtImportRequestDto
{
    public List<int> SelectedIndices { get; init; } = [];
    public bool ImportRatings { get; init; } = true;
}

public sealed record CmtImportResultDto
{
    public int ImportedCount { get; init; }
    public int SkippedCount { get; init; }
    public int ErrorCount { get; init; }
    public List<string> Errors { get; init; } = [];
}
