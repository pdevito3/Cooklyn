namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Domain.Recipes;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class GetMealPlanQueues
{
    public sealed record Query : IRequest<IReadOnlyList<MealPlanQueueDto>>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService,
        IFileStorage fileStorage) : IRequestHandler<Query, IReadOnlyList<MealPlanQueueDto>>
    {
        public async Task<IReadOnlyList<MealPlanQueueDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new Exceptions.ValidationException(nameof(MealPlanQueue), "Unable to determine tenant.");

            var queues = await dbContext.MealPlanQueues
                .AsNoTracking()
                .Include(q => q.Items.OrderBy(i => i.SortOrder))
                .OrderByDescending(q => q.IsDefault)
                .ThenBy(q => q.Name)
                .ToListAsync(cancellationToken);

            // Auto-create default queue if none exists
            if (!queues.Any(q => q.IsDefault))
            {
                var defaultQueue = MealPlanQueue.CreateDefault(tenantId);
                await dbContext.MealPlanQueues.AddAsync(defaultQueue, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                queues.Insert(0, defaultQueue);
            }

            // Batch-load recipe images for presigned URLs
            var recipeIds = queues
                .SelectMany(q => q.Items)
                .Where(i => i.RecipeId != null)
                .Select(i => i.RecipeId!)
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

            return queues.Select(q =>
            {
                var queueDto = q.ToMealPlanQueueDto();
                return queueDto with
                {
                    Items = q.Items.OrderBy(i => i.SortOrder).Select(i =>
                        i.ToMealPlanQueueItemDto(
                            i.RecipeId != null && imageUrlsByRecipeId.TryGetValue(i.RecipeId, out var url) ? url : null
                        )).ToList()
                };
            }).ToList();
        }
    }
}
