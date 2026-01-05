namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Services;

public static class UploadRecipeImage
{
    public sealed record Command(Guid RecipeId, IFormFile File) : IRequest<RecipeImageDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        IFileStorage fileStorage,
        IConfiguration configuration) : IRequestHandler<Command, RecipeImageDto>
    {
        public async Task<RecipeImageDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes.GetById(request.RecipeId, cancellationToken);

            var bucket = configuration["AWS:RecipeImagesBucket"]
                ?? throw new InvalidOperationException("AWS:RecipeImagesBucket configuration is required.");

            // Generate unique key with tenant isolation
            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            var key = $"recipes/{recipe.TenantId}/{recipe.Id}/{Guid.NewGuid()}{extension}";

            // Upload the file
            var uploadedKey = await fileStorage.UploadFileAsync(bucket, key, request.File, cancellationToken);
            ValidationException.ThrowWhenNull(uploadedKey, "Failed to upload the image.");

            // Delete old image if exists
            if (recipe.HasImage)
            {
                await fileStorage.DeleteFileAsync(recipe.ImageS3Bucket!, recipe.ImageS3Key.Value!, cancellationToken);
            }

            // Update recipe with new image info
            recipe.SetImage(bucket, uploadedKey);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Return the presigned URL
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

public sealed record RecipeImageDto
{
    public string? ImageUrl { get; init; }
    public string? ImageS3Bucket { get; init; }
    public string? ImageS3Key { get; init; }
}
