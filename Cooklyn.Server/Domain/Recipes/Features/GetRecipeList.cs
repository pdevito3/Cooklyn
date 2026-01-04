namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using Resources;

public static class GetRecipeList
{
    public sealed record Query(RecipeParametersDto Parameters) : IRequest<PagedList<RecipeSummaryDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, PagedList<RecipeSummaryDto>>
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

            var dtos = query.ToRecipeSummaryDtoQueryable();

            return await PagedList<RecipeSummaryDto>.CreateAsync(
                dtos,
                request.Parameters.PageNumber,
                request.Parameters.PageSize,
                cancellationToken);
        }
    }
}
