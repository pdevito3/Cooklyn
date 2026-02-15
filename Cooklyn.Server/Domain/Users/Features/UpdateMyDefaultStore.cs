namespace Cooklyn.Server.Domain.Users.Features;

using Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class UpdateMyDefaultStore
{
    public sealed record Command(string? StoreId) : IRequest<string?>;

    public sealed class Handler(
        AppDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<Command, string?>
    {
        public async Task<string?> Handle(Command request, CancellationToken cancellationToken)
        {
            var identifier = currentUserService.UserIdentifier;
            if (identifier is null)
                return null;

            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Identifier == identifier, cancellationToken);

            if (user is null)
                return null;

            user.UpdateDefaultStore(request.StoreId);
            await dbContext.SaveChangesAsync(cancellationToken);

            return user.DefaultStoreId;
        }
    }
}
