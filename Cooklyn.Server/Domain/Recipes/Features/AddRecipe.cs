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
        IFileStorage fileStorage) : IRequestHandler<Command, RecipeDto>
    {
        public async Task<RecipeDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(Recipe), "Unable to determine tenant.");
            var forCreation = request.Dto.ToRecipeForCreation(tenantId);
            var recipe = Recipe.Create(forCreation);

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

            await dbContext.Recipes.AddAsync(recipe, cancellationToken);
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
    }
}
