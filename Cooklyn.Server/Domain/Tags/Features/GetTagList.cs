namespace Cooklyn.Server.Domain.Tags.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using Resources;

public static class GetTagList
{
    public sealed record Query(TagParametersDto Parameters) : IRequest<PagedList<TagDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, PagedList<TagDto>>
    {
        public async Task<PagedList<TagDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryKitConfig = new CustomQueryKitConfiguration();

            IQueryable<Tag> query = dbContext.Tags.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Parameters.Filters))
                query = query.ApplyQueryKitFilter(request.Parameters.Filters, queryKitConfig);

            if (!string.IsNullOrWhiteSpace(request.Parameters.SortOrder))
                query = query.ApplyQueryKitSort(request.Parameters.SortOrder, queryKitConfig);

            var dtos = query.ToTagDtoQueryable();

            return await PagedList<TagDto>.CreateAsync(
                dtos,
                request.Parameters.PageNumber,
                request.Parameters.PageSize,
                cancellationToken);
        }
    }
}
