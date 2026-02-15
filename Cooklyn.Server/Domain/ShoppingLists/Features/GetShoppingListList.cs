namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using Resources;

public static class GetShoppingListList
{
    public sealed record Query(ShoppingListParametersDto Parameters) : IRequest<PagedList<ShoppingListSummaryDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, PagedList<ShoppingListSummaryDto>>
    {
        public async Task<PagedList<ShoppingListSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryKitConfig = new CustomQueryKitConfiguration();

            IQueryable<ShoppingList> query = dbContext.ShoppingLists
                .AsNoTracking()
                .Include(sl => sl.Items);

            if (!string.IsNullOrWhiteSpace(request.Parameters.Filters))
                query = query.ApplyQueryKitFilter(request.Parameters.Filters, queryKitConfig);

            if (!string.IsNullOrWhiteSpace(request.Parameters.SortOrder))
                query = query.ApplyQueryKitSort(request.Parameters.SortOrder, queryKitConfig);

            var count = query.Count();
            var items = await query
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = items.Select(sl => new ShoppingListSummaryDto
            {
                Id = sl.Id,
                Name = sl.Name,
                StoreId = sl.StoreId,
                Status = sl.Status.Value,
                ItemCount = sl.Items.Count,
                CheckedCount = sl.Items.Count(i => i.IsChecked),
                CompletedOn = sl.CompletedOn,
                CreatedOn = sl.CreatedOn
            }).ToList();

            return new PagedList<ShoppingListSummaryDto>(dtos, count, request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}
