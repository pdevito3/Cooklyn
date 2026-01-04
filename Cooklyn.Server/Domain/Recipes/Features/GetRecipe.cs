namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetRecipe
{
    public sealed record Query(Guid Id) : IRequest<RecipeDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, RecipeDto>
    {
        public async Task<RecipeDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes
                .Include(r => r.Ingredients.OrderBy(i => i.SortOrder))
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.Flags)
                .Include(r => r.NutritionInfo)
                .GetById(request.Id, cancellationToken);

            return recipe.ToRecipeDto();
        }
    }
}
