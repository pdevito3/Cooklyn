namespace Cooklyn.Server.Domain.SavedFilters.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using Resources;

public static class GetSavedFilterList
{
    public sealed record Query(SavedFilterParametersDto Parameters) : IRequest<PagedList<SavedFilterDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, PagedList<SavedFilterDto>>
    {
        public async Task<PagedList<SavedFilterDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryKitConfig = new CustomQueryKitConfiguration();

            IQueryable<SavedFilter> query = dbContext.SavedFilters.AsNoTracking();

            // Always filter by context
            if (!string.IsNullOrWhiteSpace(request.Parameters.Context))
                query = query.Where(sf => sf.Context == request.Parameters.Context);

            if (!string.IsNullOrWhiteSpace(request.Parameters.Filters))
                query = query.ApplyQueryKitFilter(request.Parameters.Filters, queryKitConfig);

            if (!string.IsNullOrWhiteSpace(request.Parameters.SortOrder))
                query = query.ApplyQueryKitSort(request.Parameters.SortOrder, queryKitConfig);

            var dtos = query.ToSavedFilterDtoQueryable();

            return await PagedList<SavedFilterDto>.CreateAsync(
                dtos,
                request.Parameters.PageNumber,
                request.Parameters.PageSize,
                cancellationToken);
        }
    }
}
