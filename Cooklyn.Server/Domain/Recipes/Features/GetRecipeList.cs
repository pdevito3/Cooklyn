namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using Resources;
using Services;

public static class GetRecipeList
{
    public sealed record Query(RecipeParametersDto Parameters) : IRequest<PagedList<RecipeSummaryDto>>;

    public sealed class Handler(AppDbContext dbContext, IFileStorage fileStorage) : IRequestHandler<Query, PagedList<RecipeSummaryDto>>
    {
        public async Task<PagedList<RecipeSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryKitConfig = new CustomQueryKitConfiguration();

            IQueryable<Recipe> query = dbContext.Recipes
                .AsNoTracking()
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.Flags);

            if (!string.IsNullOrWhiteSpace(request.Parameters.Filters))
                query = query.ApplyQueryKitFilter(request.Parameters.Filters, queryKitConfig);

            if (!string.IsNullOrWhiteSpace(request.Parameters.SortOrder))
                query = query.ApplyQueryKitSort(request.Parameters.SortOrder, queryKitConfig);
            else
                query = query.OrderByDescending(r => r.CreatedOn);

            // Get paginated recipes
            var pagedRecipes = await PagedList<Recipe>.CreateAsync(
                query,
                request.Parameters.PageNumber,
                request.Parameters.PageSize,
                cancellationToken);

            // Map to DTOs with presigned URLs
            var dtos = pagedRecipes.Select(r => r.ToRecipeSummaryDto(fileStorage)).ToList();

            return new PagedList<RecipeSummaryDto>(
                dtos,
                pagedRecipes.TotalCount,
                pagedRecipes.PageNumber,
                pagedRecipes.PageSize);
        }
    }
}
