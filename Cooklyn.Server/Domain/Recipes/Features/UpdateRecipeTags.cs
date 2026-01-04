namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdateRecipeTags
{
    public sealed record Command(Guid Id, IReadOnlyList<Guid> TagIds) : IRequest<RecipeDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, RecipeDto>
    {
        public async Task<RecipeDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes
                .Include(r => r.Ingredients.OrderBy(i => i.SortOrder))
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.Flags)
                .Include(r => r.NutritionInfo)
                .GetById(request.Id, cancellationToken);

            // Get the tags to set
            var tags = await dbContext.Tags
                .Where(t => request.TagIds.Contains(t.Id))
                .ToListAsync(cancellationToken);

            // Get existing recipe tag IDs for removal
            var existingRecipeTagIds = recipe.RecipeTags.Select(rt => rt.Id).ToList();

            // Set new tags
            recipe.SetTags(tags);

            // Remove old recipe tags that aren't in the new set
            var remainingIds = recipe.RecipeTags.Select(rt => rt.Id).ToHashSet();
            foreach (var existingId in existingRecipeTagIds)
            {
                if (!remainingIds.Contains(existingId))
                {
                    var recipeTag = await dbContext.RecipeTags.FindAsync(existingId);
                    if (recipeTag != null)
                        dbContext.RecipeTags.Remove(recipeTag);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return recipe.ToRecipeDto();
        }
    }
}
