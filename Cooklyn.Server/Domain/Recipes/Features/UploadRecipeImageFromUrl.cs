namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Exceptions;
using MediatR;
using Services;

public static class UploadRecipeImageFromUrl
{
    public sealed record Command(string RecipeId, string ImageUrl) : IRequest<RecipeImageDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IFileStorage fileStorage,
        IConfiguration configuration) : IRequestHandler<Command, RecipeImageDto>
    {
        private static readonly Dictionary<string, string> ContentTypeToExtension = new(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/gif"] = ".gif",
            ["image/webp"] = ".webp",
            ["image/avif"] = ".avif",
        };

        public async Task<RecipeImageDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes.GetById(request.RecipeId, cancellationToken);

            var bucket = configuration["AWS:RecipeImagesBucket"]
                ?? throw new InvalidOperationException("AWS:RecipeImagesBucket configuration is required.");

            // Download the image
            var client = httpClientFactory.CreateClient("RecipeImport");

            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, request.ImageUrl);
            httpRequest.Headers.Accept.ParseAdd("image/*,*/*;q=0.8");
            if (Uri.TryCreate(request.ImageUrl, UriKind.Absolute, out var imageUri))
            {
                httpRequest.Headers.Referrer = new Uri($"{imageUri.Scheme}://{imageUri.Host}/");
            }

            using var response = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            var extension = ContentTypeToExtension.GetValueOrDefault(contentType, ".jpg");

            // Buffer to MemoryStream since S3 upload needs a seekable/known-length stream
            await using var networkStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var memoryStream = new MemoryStream();
            await networkStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            // Generate unique key with tenant isolation
            var key = $"recipes/{recipe.TenantId}/{recipe.Id}/{Guid.NewGuid()}{extension}";

            var uploadedKey = await fileStorage.UploadFileAsync(bucket, key, memoryStream, cancellationToken);
            ValidationException.ThrowWhenNull(uploadedKey, "Failed to upload the image.");

            // Delete old image if exists
            if (recipe.HasImage)
            {
                await fileStorage.DeleteFileAsync(recipe.ImageS3Bucket!, recipe.ImageS3Key.Value!, cancellationToken);
            }

            // Update recipe with new image info
            recipe.SetImage(bucket, uploadedKey);
            await dbContext.SaveChangesAsync(cancellationToken);

            var imageUrl = recipe.GetImagePreSignedUrl(fileStorage);

            return new RecipeImageDto
            {
                ImageUrl = imageUrl,
                ImageS3Bucket = bucket,
                ImageS3Key = uploadedKey
            };
        }
    }
}
