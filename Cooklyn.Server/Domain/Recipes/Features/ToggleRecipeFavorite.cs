namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class ToggleRecipeFavorite
{
    public sealed record Command(Guid Id) : IRequest<RecipeDto>;

    public sealed class Handler(AppDbContext dbContext, IFileStorage fileStorage) : IRequestHandler<Command, RecipeDto>
    {
        public async Task<RecipeDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await dbContext.Recipes
                .Include(r => r.Ingredients.OrderBy(i => i.SortOrder))
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.Flags)
                .Include(r => r.NutritionInfo)
                .GetById(request.Id, cancellationToken);

            recipe.ToggleFavorite();

            await dbContext.SaveChangesAsync(cancellationToken);

            return recipe.ToRecipeDto(fileStorage);
        }
    }
}
