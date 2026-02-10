namespace Cooklyn.Server.Domain.Recipes.Importing;

public interface IRecipeSourceParser
{
    bool CanParse(string html, Uri url);
    ParsedRecipeData? Parse(string html, Uri url);
}

public record ParsedRecipeData
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public int? Servings { get; init; }
    public string? Steps { get; init; }
    public List<string> IngredientLines { get; init; } = [];
    public List<ParsedImageData> Images { get; init; } = [];
}

public record ParsedImageData
{
    public string Url { get; init; } = default!;
    public string? Alt { get; init; }
}
