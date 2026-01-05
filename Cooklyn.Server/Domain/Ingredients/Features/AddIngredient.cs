namespace Cooklyn.Server.Domain.Ingredients.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class AddIngredient
{
    public sealed record Command(Guid RecipeId, IngredientForCreationDto Dto) : IRequest<IngredientDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, IngredientDto>
    {
        public async Task<IngredientDto> Handle(Command request, CancellationToken cancellationToken)
        {
            // Get the current max sort order for this recipe
            var maxSortOrder = await dbContext.Ingredients
                .Where(i => i.RecipeId == request.RecipeId)
                .MaxAsync(i => (int?)i.SortOrder, cancellationToken) ?? -1;

            var forCreation = request.Dto.ToIngredientForCreation(request.RecipeId, maxSortOrder + 1);
            var ingredient = Ingredient.Create(forCreation);

            await dbContext.Ingredients.AddAsync(ingredient, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return ingredient.ToIngredientDto();
        }
    }
}
