namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class UpdateRecipeFlags
{
    public sealed record Command(Guid Id, IReadOnlyList<string> Flags) : IRequest<RecipeDto>;

    public sealed class Handler(AppDbContext dbContext, IFileStorage fileStorage) : IRequestHandler<Command, RecipeDto>
    {
        public async Task<RecipeDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.Flags)
                .Include(r => r.Ingredients)
                .Include(r => r.NutritionInfo)
                .GetById(request.Id, cancellationToken);

            // Get existing flag entry IDs for removal
            var existingFlagIds = recipe.Flags.Select(f => f.Id).ToList();

            // Convert strings to RecipeFlag value objects and set
            var flags = request.Flags.Select(RecipeFlag.Of);
            recipe.SetFlags(flags);

            // Remove old flag entries that aren't in the new set
            var remainingIds = recipe.Flags.Select(f => f.Id).ToHashSet();
            foreach (var existingId in existingFlagIds)
            {
                if (!remainingIds.Contains(existingId))
                {
                    var flagEntry = await dbContext.RecipeFlagEntries.FindAsync(existingId);
                    if (flagEntry != null)
                        dbContext.RecipeFlagEntries.Remove(flagEntry);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return recipe.ToRecipeDto(fileStorage);
        }
    }
}
