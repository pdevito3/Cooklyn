namespace Cooklyn.Server.Domain.RecentSearches.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class AddRecentSearch
{
    public sealed record Command(RecentSearchForCreationDto Dto) : IRequest<RecentSearchDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, RecentSearchDto>
    {
        private const int MaxEntriesPerTenant = 500;

        public async Task<RecentSearchDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToRecentSearchForCreation();

            // Upsert: remove existing duplicate if present
            var existing = await dbContext.RecentSearches
                .FirstOrDefaultAsync(rs =>
                    rs.SearchType == forCreation.SearchType &&
                    rs.SearchText == forCreation.SearchText &&
                    rs.ResourceType == forCreation.ResourceType &&
                    rs.ResourceId == forCreation.ResourceId,
                    cancellationToken);

            if (existing != null)
                dbContext.RecentSearches.Remove(existing);

            var recentSearch = RecentSearch.Create(forCreation);
            await dbContext.RecentSearches.AddAsync(recentSearch, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Prune old entries beyond limit
            var entriesToPrune = await dbContext.RecentSearches
                .OrderByDescending(rs => rs.CreatedOn)
                .Skip(MaxEntriesPerTenant)
                .ToListAsync(cancellationToken);

            if (entriesToPrune.Count > 0)
            {
                dbContext.RecentSearches.RemoveRange(entriesToPrune);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return recentSearch.ToRecentSearchDto();
        }
    }
}
