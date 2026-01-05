namespace Cooklyn.Server.Domain.Ingredients.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using QueryKit.Configuration;
using Resources;

public static class GetIngredientList
{
    public sealed record Query(Guid RecipeId, IngredientParametersDto Parameters) : IRequest<PagedList<IngredientDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, PagedList<IngredientDto>>
    {
        public async Task<PagedList<IngredientDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryKitConfig = new CustomQueryKitConfiguration();

            var query = dbContext.Ingredients
                .AsNoTracking()
                .Where(i => i.RecipeId == request.RecipeId)
                .ApplyQueryKitFilter(request.Parameters.Filters, queryKitConfig)
                .ApplyQueryKitSort(request.Parameters.SortOrder ?? "SortOrder", queryKitConfig);

            var dtos = query.ToIngredientDtoQueryable();

            return await PagedList<IngredientDto>.CreateAsync(
                dtos,
                request.Parameters.PageNumber,
                request.Parameters.PageSize,
                cancellationToken);
        }
    }
}
