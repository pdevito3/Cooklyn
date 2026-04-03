namespace Cooklyn.Server.Domain.Recipes.Importing.CopyMeThat;

using System.IO.Compression;
using System.Text.RegularExpressions;
using Databases;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

public interface ICmtImportService
{
    Task<CmtImportPreviewDto> ParseZipAsync(IFormFile file, CancellationToken ct);

    Task<CmtImportResultDto> ImportRecipesAsync(
        IFormFile file,
        CmtImportRequestDto request,
        CancellationToken ct);
}

public sealed partial class CmtImportService(
    AppDbContext dbContext,
    IFileStorage fileStorage,
    IConfiguration configuration,
    ILogger<CmtImportService> logger) : ICmtImportService
{
    private static readonly Dictionary<string, string> ExtensionToContentType = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".gif"] = "image/gif",
        [".webp"] = "image/webp",
        [".avif"] = "image/avif",
    };

    public async Task<CmtImportPreviewDto> ParseZipAsync(IFormFile file, CancellationToken ct)
    {
        var recipes = ParseZipFile(file);

        // Check for duplicates
        var titles = recipes.Select(r => r.Title).ToList();
        var existingTitles = await dbContext.Recipes
            .Where(r => titles.Contains(r.Title))
            .Select(r => r.Title)
            .ToListAsync(ct);

        var existingTitleSet = new HashSet<string>(existingTitles, StringComparer.OrdinalIgnoreCase);

        var previewItems = recipes.Select((r, index) =>
        {
            var isDuplicate = existingTitleSet.Contains(r.Title);
            return new CmtImportPreviewItemDto
            {
                Index = index,
                Title = r.Title,
                Source = r.Source,
                Servings = ParseServingsNumber(r.Servings),
                IngredientCount = r.IngredientLines.Count,
                Rating = r.Rating,
                HasImage = !string.IsNullOrWhiteSpace(r.ImageFileName),
                IsDuplicate = isDuplicate,
                Tags = r.Tags
            };
        }).ToList();

        return new CmtImportPreviewDto
        {
            Recipes = previewItems,
            TotalCount = previewItems.Count,
            DuplicateCount = previewItems.Count(p => p.IsDuplicate)
        };
    }

    public async Task<CmtImportResultDto> ImportRecipesAsync(
        IFormFile file,
        CmtImportRequestDto request,
        CancellationToken ct)
    {
        var recipes = ParseZipFile(file);
        var selectedSet = new HashSet<int>(request.SelectedIndices);
        var errors = new List<string>();
        var importedCount = 0;
        var skippedCount = 0;

        var bucket = configuration["AWS:RecipeImagesBucket"];

        // Open ZIP again for image extraction
        using var zipStream = file.OpenReadStream();
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var imageEntries = archive.Entries
            .Where(e => !string.IsNullOrEmpty(e.Name) && IsImageFile(e.Name))
            .ToDictionary(e => e.FullName, e => e, StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < recipes.Count; i++)
        {
            if (!selectedSet.Contains(i))
            {
                skippedCount++;
                continue;
            }

            var parsed = recipes[i];

            try
            {
                var ratingValue = request.ImportRatings && parsed.Rating.HasValue
                    ? MapCmtRating(parsed.Rating.Value)
                    : null;

                var ingredientText = string.Join("\n", parsed.IngredientLines);

                var forCreation = new RecipeForCreation
                {
                    Title = parsed.Title,
                    Description = parsed.Description,
                    Source = parsed.Source,
                    Rating = ratingValue,
                    Servings = ParseServingsNumber(parsed.Servings),
                    Steps = parsed.Steps,
                    Notes = parsed.Notes
                };

                var recipe = Recipe.Create(forCreation);
                await dbContext.Recipes.AddAsync(recipe, ct);

                // Parse and set ingredients
                if (!string.IsNullOrWhiteSpace(ingredientText))
                {
                    var ingredients = Ingredient.ParseAll(ingredientText, recipe.Id);
                    recipe.SetIngredients(ingredients);
                }

                // Upload image if available
                if (!string.IsNullOrWhiteSpace(bucket) && !string.IsNullOrWhiteSpace(parsed.ImageFileName))
                {
                    await TryUploadImage(archive, imageEntries, parsed.ImageFileName, recipe, bucket, ct);
                }

                importedCount++;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to import recipe '{Title}'", parsed.Title);
                errors.Add($"Failed to import '{parsed.Title}': {ex.Message}");
            }
        }

        await dbContext.SaveChangesAsync(ct);

        return new CmtImportResultDto
        {
            ImportedCount = importedCount,
            SkippedCount = skippedCount,
            ErrorCount = errors.Count,
            Errors = errors
        };
    }

    private static List<CmtParsedRecipe> ParseZipFile(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        // Detect format: HTML if contains recipes.html, otherwise TXT
        var htmlEntry = archive.Entries.FirstOrDefault(e =>
            e.Name.Equals("recipes.html", StringComparison.OrdinalIgnoreCase));

        List<CmtParsedRecipe> recipes;
        if (htmlEntry != null)
        {
            using var reader = new StreamReader(htmlEntry.Open());
            var html = reader.ReadToEnd();
            recipes = CmtHtmlParser.Parse(html);
        }
        else
        {
            // TXT format: parse each .txt file
            recipes = [];
            foreach (var entry in archive.Entries.Where(e =>
                         e.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) &&
                         !string.IsNullOrEmpty(e.Name)))
            {
                using var reader = new StreamReader(entry.Open());
                var content = reader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    recipes.Add(CmtTextParser.Parse(content));
                }
            }
        }

        // CMT exports newest recipes first; reverse so oldest are imported first
        // and get earlier sequential IDs for deterministic ordering
        recipes.Reverse();
        return recipes;
    }

    private async Task TryUploadImage(
        ZipArchive archive,
        Dictionary<string, ZipArchiveEntry> imageEntries,
        string imageFileName,
        Recipe recipe,
        string bucket,
        CancellationToken ct)
    {
        try
        {
            // Try exact match first, then search by filename
            ZipArchiveEntry? imageEntry = null;

            if (imageEntries.TryGetValue(imageFileName, out var exactMatch))
            {
                imageEntry = exactMatch;
            }
            else
            {
                // Try matching by just the filename portion
                var fileName = Path.GetFileName(imageFileName);
                imageEntry = imageEntries.Values
                    .FirstOrDefault(e => e.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));
            }

            if (imageEntry == null)
                return;

            await using var imageStream = imageEntry.Open();
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;

            var extension = Path.GetExtension(imageEntry.Name).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
                extension = ".jpg";

            var key = $"recipes/{recipe.Id}/{Guid.NewGuid()}{extension}";
            var uploadedKey = await fileStorage.UploadFileAsync(bucket, key, memoryStream, ct);
            if (uploadedKey != null)
            {
                recipe.SetImage(bucket, uploadedKey);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to upload image '{ImageFileName}' for recipe '{RecipeTitle}'",
                imageFileName, recipe.Title);
        }
    }

    private static string? MapCmtRating(int cmtRating)
    {
        return cmtRating switch
        {
            5 => "Loved It",
            4 => "Liked It",
            3 => "It Was Ok",
            2 => "Not Great",
            1 => "Hated It",
            _ => null
        };
    }

    private static int? ParseServingsNumber(string? servingsText)
    {
        if (string.IsNullOrWhiteSpace(servingsText))
            return null;

        var match = LeadingNumberPattern().Match(servingsText);
        if (match.Success && int.TryParse(match.Value, out var n))
            return n;

        return null;
    }

    private static bool IsImageFile(string fileName)
    {
        var ext = Path.GetExtension(fileName);
        return ExtensionToContentType.ContainsKey(ext);
    }

    [GeneratedRegex(@"^\d+")]
    private static partial Regex LeadingNumberPattern();
}
