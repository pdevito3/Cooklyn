namespace Cooklyn.Server.Domain.Recipes.Importing;

using Dtos;

public interface IRecipeImportService
{
    Task<ImportRecipePreviewDto> ImportFromUrlAsync(string url, CancellationToken cancellationToken = default);
}

public class RecipeImportService(
    IHttpClientFactory httpClientFactory,
    IEnumerable<IRecipeSourceParser> parsers) : IRecipeImportService
{
    public async Task<ImportRecipePreviewDto> ImportFromUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != "http" && uri.Scheme != "https"))
        {
            throw new Exceptions.ValidationException("Url", "Please provide a valid HTTP or HTTPS URL.");
        }

        var client = httpClientFactory.CreateClient("RecipeImport");
        var html = await client.GetStringAsync(uri, cancellationToken);

        ParsedRecipeData? parsed = null;
        foreach (var parser in parsers)
        {
            if (parser.CanParse(html, uri))
            {
                parsed = parser.Parse(html, uri);
                if (parsed != null)
                    break;
            }
        }

        if (parsed == null)
        {
            throw new Exceptions.ValidationException("Url",
                "Could not find recipe data on this page. The site may not include structured recipe data.");
        }

        // Parse ingredient lines into structured DTOs
        var ingredientText = string.Join("\n", parsed.IngredientLines);
        var ingredients = Ingredient.ParseAll(ingredientText, string.Empty);
        var ingredientDtos = ingredients.Select(i => new IngredientForCreationDto
        {
            RawText = i.RawText,
            Name = i.Name,
            Amount = i.Amount,
            AmountText = i.AmountText,
            Unit = i.Unit.Value,
            CustomUnit = i.CustomUnit,
            GroupName = i.GroupName,
            SortOrder = i.SortOrder
        }).ToList();

        var imageDtos = parsed.Images.Select(img => new ImportImageDto
        {
            Url = img.Url,
            Alt = img.Alt
        }).ToList();

        return new ImportRecipePreviewDto
        {
            Title = parsed.Title,
            Description = parsed.Description,
            Source = url,
            Servings = parsed.Servings,
            Steps = parsed.Steps,
            Ingredients = ingredientDtos,
            Images = imageDtos
        };
    }
}
