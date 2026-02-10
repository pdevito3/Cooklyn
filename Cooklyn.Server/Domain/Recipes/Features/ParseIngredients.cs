namespace Cooklyn.Server.Domain.Recipes.Features;

using Dtos;
using MediatR;

public static class ParseIngredients
{
    public sealed record Command(string Text) : IRequest<IReadOnlyList<IngredientForCreationDto>>;

    public sealed class Handler : IRequestHandler<Command, IReadOnlyList<IngredientForCreationDto>>
    {
        public Task<IReadOnlyList<IngredientForCreationDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var ingredients = Ingredient.ParseAll(request.Text, string.Empty);
            var result = ingredients.Select(i => new IngredientForCreationDto
            {
                RawText = i.RawText,
                Name = i.Name,
                Amount = i.Amount,
                AmountText = i.AmountText,
                Unit = i.Unit.Value,
                CustomUnit = i.CustomUnit,
                GroupName = i.GroupName,
                SortOrder = i.SortOrder
            }).ToList() as IReadOnlyList<IngredientForCreationDto>;

            return Task.FromResult(result);
        }
    }
}
