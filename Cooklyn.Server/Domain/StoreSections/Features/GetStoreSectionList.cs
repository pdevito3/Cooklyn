namespace Cooklyn.Server.Domain.StoreSections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using Resources;

public static class GetStoreSectionList
{
    public sealed record Query(StoreSectionParametersDto Parameters) : IRequest<PagedList<StoreSectionDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, PagedList<StoreSectionDto>>
    {
        public async Task<PagedList<StoreSectionDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryKitConfig = new CustomQueryKitConfiguration();

            IQueryable<StoreSection> query = dbContext.StoreSections.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Parameters.Filters))
                query = query.ApplyQueryKitFilter(request.Parameters.Filters, queryKitConfig);

            if (!string.IsNullOrWhiteSpace(request.Parameters.SortOrder))
                query = query.ApplyQueryKitSort(request.Parameters.SortOrder, queryKitConfig);

            var dtos = query.ToStoreSectionDtoQueryable();

            return await PagedList<StoreSectionDto>.CreateAsync(
                dtos,
                request.Parameters.PageNumber,
                request.Parameters.PageSize,
                cancellationToken);
        }
    }
}
