namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class UpdateRecipe
{
    public sealed record Command(string Id, RecipeForUpdateDto Dto) : IRequest<RecipeDto>;

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

            var forUpdate = request.Dto.ToRecipeForUpdate();
            recipe.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return recipe.ToRecipeDto(fileStorage);
        }
    }
}
