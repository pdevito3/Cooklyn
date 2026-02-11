namespace Cooklyn.Server.Domain.Recipes.Importing.CopyMeThat;

public sealed record CmtParsedRecipe
{
    public string Title { get; init; } = default!;
    public string? Source { get; init; }
    public string? Description { get; init; }
    public string? Servings { get; init; }
    public List<string> IngredientLines { get; init; } = [];
    public string? Steps { get; init; }
    public string? Notes { get; init; }
    public int? Rating { get; init; }
    public List<string> Tags { get; init; } = [];
    public string? ImageFileName { get; init; }
}
