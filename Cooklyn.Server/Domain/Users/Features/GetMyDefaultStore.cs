namespace Cooklyn.Server.Domain.Users.Features;

using Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class GetMyDefaultStore
{
    public sealed record Query : IRequest<string?>;

    public sealed class Handler(
        AppDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<Query, string?>
    {
        public async Task<string?> Handle(Query request, CancellationToken cancellationToken)
        {
            var identifier = currentUserService.UserIdentifier;
            if (identifier is null)
                return null;

            var defaultStoreId = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Identifier == identifier)
                .Select(u => u.DefaultStoreId)
                .FirstOrDefaultAsync(cancellationToken);

            return defaultStoreId;
        }
    }
}
