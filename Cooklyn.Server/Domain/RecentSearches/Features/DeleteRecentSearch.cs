namespace Cooklyn.Server.Domain.RecentSearches.Features;

using Databases;
using MediatR;

public static class DeleteRecentSearch
{
    public sealed record Command(string Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recentSearch = await dbContext.RecentSearches.GetById(request.Id, cancellationToken);

            dbContext.RecentSearches.Remove(recentSearch);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
