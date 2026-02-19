namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetMealPlanCalendarIngredients
{
    public sealed record Query(DateOnly StartDate, DateOnly EndDate)
        : IRequest<IReadOnlyList<MealPlanRecipeIngredientsDto>>;

    public sealed class Handler(AppDbContext dbContext)
        : IRequestHandler<Query, IReadOnlyList<MealPlanRecipeIngredientsDto>>
    {
        public async Task<IReadOnlyList<MealPlanRecipeIngredientsDto>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var entries = await dbContext.MealPlanEntries
                .AsNoTracking()
                .Where(e => e.Date >= request.StartDate
                    && e.Date <= request.EndDate
                    && e.EntryType.Value == "Recipe"
                    && e.RecipeId != null)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.SortOrder)
                .ToListAsync(cancellationToken);

            if (entries.Count == 0)
                return [];

            var recipeIds = entries.Select(e => e.RecipeId!).Distinct().ToList();
            var recipes = await dbContext.Recipes
                .AsNoTracking()
                .Include(r => r.Ingredients)
                .Where(r => recipeIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, cancellationToken);

            return entries
                .Where(e => recipes.ContainsKey(e.RecipeId!))
                .Select(e =>
                {
                    var recipe = recipes[e.RecipeId!];
                    return new MealPlanRecipeIngredientsDto
                    {
                        EntryId = e.Id,
                        RecipeId = recipe.Id,
                        RecipeTitle = recipe.Title,
                        Date = e.Date,
                        Scale = e.Scale,
                        Ingredients = recipe.Ingredients
                            .OrderBy(i => i.SortOrder)
                            .Select(i => new MealPlanIngredientDto
                            {
                                Id = i.Id,
                                Name = i.Name,
                                RawText = i.RawText,
                                Amount = i.Amount,
                                AmountText = i.AmountText,
                                Unit = i.Unit.Value
                            })
                            .ToList()
                    };
                })
                .ToList();
        }
    }
}
