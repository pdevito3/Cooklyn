namespace Cooklyn.Server.Domain.Ingredients.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class GetIngredient
{
    public sealed record Query(Guid Id) : IRequest<IngredientDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, IngredientDto>
    {
        public async Task<IngredientDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var ingredient = await dbContext.Ingredients.GetById(request.Id, cancellationToken);
            return ingredient.ToIngredientDto();
        }
    }
}
