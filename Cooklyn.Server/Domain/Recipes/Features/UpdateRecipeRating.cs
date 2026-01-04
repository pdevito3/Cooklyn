namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdateRecipeRating
{
    public sealed record Command(Guid Id, string Rating) : IRequest<RecipeDto>;

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

            var rating = Rating.Of(request.Rating);
            recipe.UpdateRating(rating);

            await dbContext.SaveChangesAsync(cancellationToken);

            return recipe.ToRecipeDto();
        }
    }
}
