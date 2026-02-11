namespace Cooklyn.Server.Domain.Recipes.Importing.CopyMeThat;

using AngleSharp;
using AngleSharp.Html.Parser;

public static class CmtHtmlParser
{
    public static List<CmtParsedRecipe> Parse(string html)
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument(html);
        var recipes = new List<CmtParsedRecipe>();

        var recipeElements = document.QuerySelectorAll("div.recipe");

        foreach (var el in recipeElements)
        {
            var title = el.QuerySelector("#name")?.TextContent.Trim();
            if (string.IsNullOrWhiteSpace(title))
                continue;

            var sourceLink = el.QuerySelector("#original_link");
            var source = sourceLink?.GetAttribute("href");

            var imageEl = el.QuerySelector(".recipeImage");
            var imageSrc = imageEl?.GetAttribute("src");

            var categoryEls = el.QuerySelectorAll(".recipeCategory");
            var tags = categoryEls
                .Select(c => c.TextContent.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            var description = el.QuerySelector("#description")?.TextContent.Trim();
            if (string.IsNullOrWhiteSpace(description))
                description = null;

            var ratingText = el.QuerySelector("#ratingValue")?.TextContent.Trim();
            int? rating = null;
            if (int.TryParse(ratingText, out var r) && r is >= 1 and <= 5)
                rating = r;

            var yieldEl = el.QuerySelector("#recipeYield");
            var servings = yieldEl?.TextContent.Trim();
            if (string.IsNullOrWhiteSpace(servings))
                servings = null;

            // Ingredients: items and subheaders
            var ingredientLines = new List<string>();
            var ingredientContainer = el.QuerySelector(".recipeIngredients");
            if (ingredientContainer != null)
            {
                foreach (var child in ingredientContainer.Children)
                {
                    if (child.ClassList.Contains("recipeIngredient_subheader"))
                    {
                        var groupName = child.TextContent.Trim();
                        if (!string.IsNullOrWhiteSpace(groupName))
                            ingredientLines.Add(groupName.TrimEnd(':') + ":");
                    }
                    else if (child.ClassList.Contains("recipeIngredient"))
                    {
                        var text = child.TextContent.Trim();
                        if (!string.IsNullOrWhiteSpace(text))
                            ingredientLines.Add(text);
                    }
                }
            }
            else
            {
                // Fallback: look for list items
                var ingredientItems = el.QuerySelectorAll(".recipeIngredient");
                foreach (var item in ingredientItems)
                {
                    var text = item.TextContent.Trim();
                    if (!string.IsNullOrWhiteSpace(text))
                        ingredientLines.Add(text);
                }
            }

            // Steps
            var stepItems = el.QuerySelectorAll(".instruction");
            var stepTexts = stepItems
                .Select(s => s.TextContent.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            string? steps = stepTexts.Count > 0 ? string.Join("\n", stepTexts) : null;

            // Notes
            var noteItems = el.QuerySelectorAll(".recipeNote");
            var noteTexts = noteItems
                .Select(n => n.TextContent.Trim())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();
            string? notes = noteTexts.Count > 0 ? string.Join("\n", noteTexts) : null;

            recipes.Add(new CmtParsedRecipe
            {
                Title = title,
                Source = source,
                Description = description,
                Servings = servings,
                IngredientLines = ingredientLines,
                Steps = steps,
                Notes = notes,
                Rating = rating,
                Tags = tags,
                ImageFileName = imageSrc
            });
        }

        return recipes;
    }
}
