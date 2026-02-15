namespace Cooklyn.Server.Domain.ItemCollections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using Resources;

public static class GetItemCollectionList
{
    public sealed record Query(ItemCollectionParametersDto Parameters) : IRequest<PagedList<ItemCollectionDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, PagedList<ItemCollectionDto>>
    {
        public async Task<PagedList<ItemCollectionDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryKitConfig = new CustomQueryKitConfiguration();

            IQueryable<ItemCollection> query = dbContext.ItemCollections
                .AsNoTracking()
                .Include(ic => ic.Items);

            if (!string.IsNullOrWhiteSpace(request.Parameters.Filters))
                query = query.ApplyQueryKitFilter(request.Parameters.Filters, queryKitConfig);

            if (!string.IsNullOrWhiteSpace(request.Parameters.SortOrder))
                query = query.ApplyQueryKitSort(request.Parameters.SortOrder, queryKitConfig);

            var count = query.Count();
            var items = await query
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = items.Select(ic => ic.ToItemCollectionDto()).ToList();

            return new PagedList<ItemCollectionDto>(dtos, count, request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}
