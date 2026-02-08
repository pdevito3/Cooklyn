namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using MediatR;
using Services;

public static class DeleteRecipeImage
{
    public sealed record Command(Guid RecipeId) : IRequest;

    public sealed class Handler(
        AppDbContext dbContext,
        IFileStorage fileStorage) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes.GetById(request.RecipeId, cancellationToken);

            if (recipe.HasImage)
            {
                await fileStorage.DeleteFileAsync(recipe.ImageS3Bucket!, recipe.ImageS3Key.Value!, cancellationToken);
            }

            recipe.ClearImage();
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
