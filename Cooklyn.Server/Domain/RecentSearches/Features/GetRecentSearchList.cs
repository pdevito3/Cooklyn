namespace Cooklyn.Server.Domain.RecentSearches.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetRecentSearchList
{
    public sealed record Query(RecentSearchParametersDto Parameters) : IRequest<List<RecentSearchDto>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, List<RecentSearchDto>>
    {
        public async Task<List<RecentSearchDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var pageSize = request.Parameters.GetEffectivePageSize();

            return await dbContext.RecentSearches
                .AsNoTracking()
                .OrderByDescending(rs => rs.CreatedOn)
                .Take(pageSize)
                .Select(rs => rs.ToRecentSearchDto())
                .ToListAsync(cancellationToken);
        }
    }
}
