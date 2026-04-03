namespace Cooklyn.Server.Domain.RecentSearches.Features;

using Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class ClearRecentSearches
{
    public sealed record Command : IRequest;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await dbContext.RecentSearches
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
