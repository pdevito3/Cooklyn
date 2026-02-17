namespace Cooklyn.Server.Domain.Stores.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using Resources;

public static class GetStoreList
{
    public sealed record Query(StoreParametersDto Parameters) : IRequest<PagedList<StoreDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, PagedList<StoreDto>>
    {
        public async Task<PagedList<StoreDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryKitConfig = new CustomQueryKitConfiguration();

            IQueryable<Store> query = dbContext.Stores
                .AsNoTracking()
                .Include(s => s.StoreAisles)
                .Include(s => s.StoreDefaultCollections)
                    .ThenInclude(sdc => sdc.ItemCollection);

            if (!string.IsNullOrWhiteSpace(request.Parameters.Filters))
                query = query.ApplyQueryKitFilter(request.Parameters.Filters, queryKitConfig);

            if (!string.IsNullOrWhiteSpace(request.Parameters.SortOrder))
                query = query.ApplyQueryKitSort(request.Parameters.SortOrder, queryKitConfig);

            var count = query.Count();
            var items = await query
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = items.Select(s => s.ToStoreDto()).ToList();

            return new PagedList<StoreDto>(dtos, count, request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}
