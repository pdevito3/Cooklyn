namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Domain.Recipes;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class GetMealPlanCalendar
{
    public sealed record Query(DateOnly StartDate, DateOnly EndDate) : IRequest<IReadOnlyList<MealPlanDayDto>>;

    public sealed class Handler(AppDbContext dbContext, IFileStorage fileStorage) : IRequestHandler<Query, IReadOnlyList<MealPlanDayDto>>
    {
        public async Task<IReadOnlyList<MealPlanDayDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var entries = await dbContext.MealPlanEntries
                .AsNoTracking()
                .Where(e => e.Date >= request.StartDate && e.Date <= request.EndDate)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.SortOrder)
                .ToListAsync(cancellationToken);

            // Batch-load recipe images for presigned URLs
            var recipeIds = entries
                .Where(e => e.RecipeId != null)
                .Select(e => e.RecipeId!)
                .Distinct()
                .ToList();

            var imageUrlsByRecipeId = new Dictionary<string, string?>();
            if (recipeIds.Count > 0)
            {
                var recipes = await dbContext.Recipes
                    .AsNoTracking()
                    .Where(r => recipeIds.Contains(r.Id))
                    .ToListAsync(cancellationToken);

                foreach (var recipe in recipes)
                {
                    imageUrlsByRecipeId[recipe.Id] = recipe.GetImagePreSignedUrl(fileStorage);
                }
            }

            var days = entries
                .GroupBy(e => e.Date)
                .Select(g => new MealPlanDayDto
                {
                    Date = g.Key,
                    Entries = g.Select(e => e.ToMealPlanEntryDto(
                        e.RecipeId != null && imageUrlsByRecipeId.TryGetValue(e.RecipeId, out var url) ? url : null
                    )).ToList()
                })
                .ToList();

            return days.OrderBy(d => d.Date).ToList();
        }
    }
}
