namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

public static class AddRecipe
{
    public sealed record Command(RecipeForCreationDto Dto) : IRequest<RecipeDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService,
        IFileStorage fileStorage,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<Handler> logger) : IRequestHandler<Command, RecipeDto>
    {
        private static readonly Dictionary<string, string> ContentTypeToExtension = new(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/gif"] = ".gif",
            ["image/webp"] = ".webp",
            ["image/avif"] = ".avif",
        };

        public async Task<RecipeDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(Recipe), "Unable to determine tenant.");
            var forCreation = request.Dto.ToRecipeForCreation(tenantId);
            var recipe = Recipe.Create(forCreation);

            // Track entity so the value generator assigns the Id immediately
            await dbContext.Recipes.AddAsync(recipe, cancellationToken);

            // Add tags
            if (request.Dto.TagIds.Any())
            {
                var tags = await dbContext.Tags
                    .Where(t => request.Dto.TagIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                foreach (var tag in tags)
                {
                    recipe.AddTag(tag);
                }
            }

            // Add flags
            foreach (var flagValue in request.Dto.Flags)
            {
                var flag = RecipeFlag.Of(flagValue);
                recipe.AddFlag(flag);
            }

            // Add ingredients
            if (request.Dto.Ingredients.Any())
            {
                var ingredients = request.Dto.Ingredients.Select(dto =>
                    Ingredient.Create(dto.ToIngredientForCreation(recipe.Id)));
                recipe.SetIngredients(ingredients);
            }

            // Add nutrition info if provided
            if (request.Dto.NutritionInfo != null)
            {
                var nutritionForCreation = request.Dto.NutritionInfo.ToNutritionInfoForCreation(recipe.Id);
                recipe.SetNutritionInfo(nutritionForCreation);
            }

            // Download and upload image from external URL if provided
            if (!string.IsNullOrWhiteSpace(request.Dto.ImageUrl))
            {
                await DownloadAndAttachImage(recipe, request.Dto.ImageUrl, cancellationToken);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            // Reload with all navigations for the response
            var loadedRecipe = await dbContext.Recipes
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.Flags)
                .Include(r => r.Ingredients)
                .Include(r => r.NutritionInfo)
                .FirstAsync(r => r.Id == recipe.Id, cancellationToken);

            return loadedRecipe.ToRecipeDto(fileStorage);
        }

        private async Task DownloadAndAttachImage(Recipe recipe, string imageUrl, CancellationToken cancellationToken)
        {
            var bucket = configuration["AWS:RecipeImagesBucket"];
            if (string.IsNullOrEmpty(bucket))
                return;

            try
            {
                var client = httpClientFactory.CreateClient("RecipeImport");

                using var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);
                request.Headers.Accept.ParseAdd("image/*,*/*;q=0.8");
                if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var imageUri))
                {
                    request.Headers.Referrer = new Uri($"{imageUri.Scheme}://{imageUri.Host}/");
                }

                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
                var extension = ContentTypeToExtension.GetValueOrDefault(contentType, ".jpg");

                // Buffer to MemoryStream since S3 upload needs a seekable/known-length stream for some providers
                await using var networkStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var memoryStream = new MemoryStream();
                await networkStream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                var key = $"recipes/{recipe.TenantId}/{recipe.Id}/{Guid.NewGuid()}{extension}";

                var uploadedKey = await fileStorage.UploadFileAsync(bucket, key, memoryStream, cancellationToken);
                if (uploadedKey != null)
                {
                    recipe.SetImage(bucket, uploadedKey);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to download and attach image from URL {ImageUrl} for recipe {RecipeId}",
                    imageUrl, recipe.Id);
            }
        }
    }
}
