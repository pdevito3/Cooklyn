namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class UpdateRecipeIngredients
{
    public sealed record Command(string Id, IReadOnlyList<IngredientForCreationDto> Ingredients) : IRequest<RecipeDto>;

    public sealed class Handler(AppDbContext dbContext, IFileStorage fileStorage) : IRequestHandler<Command, RecipeDto>
    {
        public async Task<RecipeDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes
                .Include(r => r.Ingredients)
                .GetById(request.Id, cancellationToken);

            var existing = recipe.Ingredients.OrderBy(i => i.SortOrder).ToList();
            var incoming = request.Ingredients;

            // Match by position: update existing, add new, remove extras
            for (var i = 0; i < incoming.Count; i++)
            {
                var dto = incoming[i];
                if (i < existing.Count)
                {
                    // UPDATE existing entity in-place (preserves Id)
                    existing[i].Update(dto.ToIngredientForUpdate());
                }
                else
                {
                    // INSERT new entity
                    var ingredient = Ingredient.Create(dto.ToIngredientForCreation(recipe.Id));
                    await dbContext.Ingredients.AddAsync(ingredient, cancellationToken);
                }
            }

            // DELETE extras beyond new list length
            if (existing.Count > incoming.Count)
            {
                dbContext.Ingredients.RemoveRange(existing.Skip(incoming.Count));
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            // Reload with all navigations for the response
            var loadedRecipe = await dbContext.Recipes
                .AsNoTracking()
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.Flags)
                .Include(r => r.Ingredients)
                .Include(r => r.NutritionInfo)
                .FirstAsync(r => r.Id == recipe.Id, cancellationToken);

            return loadedRecipe.ToRecipeDto(fileStorage);
        }
    }
}
