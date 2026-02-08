namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class DeleteRecipe
{
    public sealed record Command(Guid Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext, IFileStorage fileStorage) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes
                .Include(r => r.RecipeTags)
                .Include(r => r.Flags)
                .Include(r => r.NutritionInfo)
                .GetById(request.Id, cancellationToken);

            // Delete S3 image if one exists
            if (recipe.HasImage)
            {
                await fileStorage.DeleteFileAsync(recipe.ImageS3Bucket!, recipe.ImageS3Key.Value!, cancellationToken);
            }

            // Remove all related entities first due to PropertyAccessMode.Field
            foreach (var recipeTag in recipe.RecipeTags.ToList())
            {
                dbContext.RecipeTags.Remove(recipeTag);
            }

            foreach (var flag in recipe.Flags.ToList())
            {
                dbContext.RecipeFlagEntries.Remove(flag);
            }

            if (recipe.NutritionInfo != null)
            {
                dbContext.NutritionInfos.Remove(recipe.NutritionInfo);
            }

            dbContext.Recipes.Remove(recipe);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
