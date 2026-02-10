namespace Cooklyn.Server.Domain.Recipes.Importing;

using System.Text.Json;
using System.Text.RegularExpressions;

public partial class JsonLdRecipeParser : IRecipeSourceParser
{
    [GeneratedRegex("""<script[^>]+type\s*=\s*["']application/ld\+json["'][^>]*>(.*?)</script>""",
        RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex JsonLdScriptPattern();

    [GeneratedRegex("""<img[^>]+src\s*=\s*["']([^"']+)["'][^>]*>""",
        RegexOptions.IgnoreCase)]
    private static partial Regex ImgTagPattern();

    [GeneratedRegex("""alt\s*=\s*["']([^"']*)["']""", RegexOptions.IgnoreCase)]
    private static partial Regex ImgAltPattern();

    public bool CanParse(string html, Uri url)
    {
        var matches = JsonLdScriptPattern().Matches(html);
        foreach (Match match in matches)
        {
            var json = match.Groups[1].Value.Trim();
            if (TryFindRecipeNode(json) != null)
                return true;
        }

        return false;
    }

    public ParsedRecipeData? Parse(string html, Uri url)
    {
        JsonElement? recipeNode = null;

        var matches = JsonLdScriptPattern().Matches(html);
        foreach (Match match in matches)
        {
            var json = match.Groups[1].Value.Trim();
            recipeNode = TryFindRecipeNode(json);
            if (recipeNode != null)
                break;
        }

        if (recipeNode is not { } recipe)
            return null;

        var images = ExtractImages(recipe, html, url);
        var ingredientLines = ExtractStringArray(recipe, "recipeIngredient");
        var steps = ExtractInstructions(recipe);
        var servings = ExtractServings(recipe);

        return new ParsedRecipeData
        {
            Title = GetStringProp(recipe, "name"),
            Description = GetStringProp(recipe, "description"),
            Servings = servings,
            Steps = steps,
            IngredientLines = ingredientLines,
            Images = images
        };
    }

    private static JsonElement? TryFindRecipeNode(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Direct Recipe object
            if (IsRecipeType(root))
                return root;

            // Array of objects (some sites emit an array at the top level)
            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in root.EnumerateArray())
                {
                    if (IsRecipeType(item))
                        return item;
                }
            }

            // @graph pattern
            if (root.TryGetProperty("@graph", out var graph) && graph.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in graph.EnumerateArray())
                {
                    if (IsRecipeType(item))
                        return item;
                }
            }
        }
        catch (JsonException)
        {
            // Invalid JSON, skip
        }

        return null;
    }

    private static bool IsRecipeType(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return false;

        if (!element.TryGetProperty("@type", out var typeProp))
            return false;

        if (typeProp.ValueKind == JsonValueKind.String)
            return typeProp.GetString()?.Equals("Recipe", StringComparison.OrdinalIgnoreCase) == true;

        if (typeProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var t in typeProp.EnumerateArray())
            {
                if (t.ValueKind == JsonValueKind.String &&
                    t.GetString()?.Equals("Recipe", StringComparison.OrdinalIgnoreCase) == true)
                    return true;
            }
        }

        return false;
    }

    private static string? GetStringProp(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
            return prop.GetString();
        return null;
    }

    private static List<string> ExtractStringArray(JsonElement element, string propertyName)
    {
        var result = new List<string>();

        if (!element.TryGetProperty(propertyName, out var prop))
            return result;

        if (prop.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in prop.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var val = item.GetString();
                    if (!string.IsNullOrWhiteSpace(val))
                        result.Add(val.Trim());
                }
            }
        }
        else if (prop.ValueKind == JsonValueKind.String)
        {
            var val = prop.GetString();
            if (!string.IsNullOrWhiteSpace(val))
                result.Add(val.Trim());
        }

        return result;
    }

    private static int? ExtractServings(JsonElement recipe)
    {
        if (!recipe.TryGetProperty("recipeYield", out var yield))
            return null;

        string? yieldStr = null;

        if (yield.ValueKind == JsonValueKind.String)
        {
            yieldStr = yield.GetString();
        }
        else if (yield.ValueKind == JsonValueKind.Number)
        {
            return yield.TryGetInt32(out var n) ? n : null;
        }
        else if (yield.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in yield.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    yieldStr = item.GetString();
                    break;
                }
                if (item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out var n))
                    return n;
            }
        }

        if (yieldStr == null)
            return null;

        // Try to extract a number from the string (e.g. "4 servings" → 4)
        var numMatch = Regex.Match(yieldStr, @"\d+");
        return numMatch.Success && int.TryParse(numMatch.Value, out var parsed) ? parsed : null;
    }

    private static string? ExtractInstructions(JsonElement recipe)
    {
        if (!recipe.TryGetProperty("recipeInstructions", out var instructions))
            return null;

        var steps = new List<string>();

        if (instructions.ValueKind == JsonValueKind.String)
        {
            return instructions.GetString();
        }

        if (instructions.ValueKind == JsonValueKind.Array)
        {
            var stepNumber = 1;
            foreach (var item in instructions.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var text = item.GetString();
                    if (!string.IsNullOrWhiteSpace(text))
                        steps.Add($"{stepNumber++}. {text.Trim()}");
                }
                else if (item.ValueKind == JsonValueKind.Object)
                {
                    // HowToStep
                    var text = GetStringProp(item, "text");
                    if (!string.IsNullOrWhiteSpace(text))
                        steps.Add($"{stepNumber++}. {text.Trim()}");
                }
            }
        }

        return steps.Count > 0 ? string.Join("\n\n", steps) : null;
    }

    private static List<ParsedImageData> ExtractImages(JsonElement recipe, string html, Uri baseUrl)
    {
        var images = new List<ParsedImageData>();
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Extract from JSON-LD image field first (higher priority)
        if (recipe.TryGetProperty("image", out var imageProp))
        {
            ExtractJsonLdImages(imageProp, baseUrl, images, seenUrls);
        }

        // Extract from HTML img tags
        var imgMatches = ImgTagPattern().Matches(html);
        foreach (Match match in imgMatches)
        {
            var src = match.Groups[1].Value;
            var resolvedUrl = ResolveUrl(src, baseUrl);
            if (resolvedUrl == null || !seenUrls.Add(resolvedUrl))
                continue;

            // Skip tiny images (icons, spacers, etc.)
            if (IsLikelyNonContentImage(src))
                continue;

            string? alt = null;
            var altMatch = ImgAltPattern().Match(match.Value);
            if (altMatch.Success)
                alt = altMatch.Groups[1].Value;

            images.Add(new ParsedImageData { Url = resolvedUrl, Alt = alt });
        }

        return images;
    }

    private static void ExtractJsonLdImages(JsonElement imageProp, Uri baseUrl,
        List<ParsedImageData> images, HashSet<string> seenUrls)
    {
        if (imageProp.ValueKind == JsonValueKind.String)
        {
            var url = ResolveUrl(imageProp.GetString(), baseUrl);
            if (url != null && seenUrls.Add(url))
                images.Add(new ParsedImageData { Url = url });
        }
        else if (imageProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in imageProp.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var url = ResolveUrl(item.GetString(), baseUrl);
                    if (url != null && seenUrls.Add(url))
                        images.Add(new ParsedImageData { Url = url });
                }
                else if (item.ValueKind == JsonValueKind.Object)
                {
                    ExtractImageObject(item, baseUrl, images, seenUrls);
                }
            }
        }
        else if (imageProp.ValueKind == JsonValueKind.Object)
        {
            ExtractImageObject(imageProp, baseUrl, images, seenUrls);
        }
    }

    private static void ExtractImageObject(JsonElement imageObj, Uri baseUrl,
        List<ParsedImageData> images, HashSet<string> seenUrls)
    {
        var url = GetStringProp(imageObj, "url") ?? GetStringProp(imageObj, "contentUrl");
        if (url == null)
            return;

        var resolvedUrl = ResolveUrl(url, baseUrl);
        if (resolvedUrl == null || !seenUrls.Add(resolvedUrl))
            return;

        images.Add(new ParsedImageData
        {
            Url = resolvedUrl,
            Alt = GetStringProp(imageObj, "caption") ?? GetStringProp(imageObj, "name")
        });
    }

    private static string? ResolveUrl(string? url, Uri baseUrl)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        // data: URIs are not useful
        if (url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return null;

        if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
            return absoluteUri.ToString();

        if (Uri.TryCreate(baseUrl, url, out var resolvedUri))
            return resolvedUri.ToString();

        return null;
    }

    private static bool IsLikelyNonContentImage(string src)
    {
        var lower = src.ToLowerInvariant();
        return lower.Contains("1x1") ||
               lower.Contains("pixel") ||
               lower.Contains("spacer") ||
               lower.Contains("tracking") ||
               lower.Contains(".svg") ||
               lower.Contains("data:image");
    }
}
