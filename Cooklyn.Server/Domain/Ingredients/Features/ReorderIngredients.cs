namespace Cooklyn.Server.Domain.Ingredients.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class ReorderIngredients
{
    public sealed record Command(Guid RecipeId, ReorderIngredientsDto Dto) : IRequest<IReadOnlyList<IngredientDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, IReadOnlyList<IngredientDto>>
    {
        public async Task<IReadOnlyList<IngredientDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var ingredients = await dbContext.Ingredients
                .Where(i => i.RecipeId == request.RecipeId)
                .ToListAsync(cancellationToken);

            var orderedIds = request.Dto.IngredientIds.ToList();
            for (var i = 0; i < orderedIds.Count; i++)
            {
                var ingredient = ingredients.FirstOrDefault(ing => ing.Id == orderedIds[i]);
                ingredient?.UpdateSortOrder(i);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return ingredients
                .OrderBy(i => i.SortOrder)
                .Select(i => i.ToIngredientDto())
                .ToList();
        }
    }
}
