namespace Cooklyn.Server.Domain.SavedFilters.Features;

using Databases;
using MediatR;

public static class DeleteSavedFilter
{
    public sealed record Command(string Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var savedFilter = await dbContext.SavedFilters.GetById(request.Id, cancellationToken);

            dbContext.SavedFilters.Remove(savedFilter);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
