namespace Cooklyn.Server.Domain.RecentSearches.Features;

using Databases;
using Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class ClearRecentSearches
{
    public sealed record Command : IRequest;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(RecentSearch), "Unable to determine tenant.");

            await dbContext.RecentSearches
                .Where(rs => rs.TenantId == tenantId)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
