namespace Cooklyn.Server.Domain.Ingredients.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class UpdateIngredient
{
    public sealed record Command(Guid Id, IngredientForUpdateDto Dto) : IRequest<IngredientDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, IngredientDto>
    {
        public async Task<IngredientDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var ingredient = await dbContext.Ingredients.GetById(request.Id, cancellationToken);

            var forUpdate = request.Dto.ToIngredientForUpdate();
            ingredient.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return ingredient.ToIngredientDto();
        }
    }
}
