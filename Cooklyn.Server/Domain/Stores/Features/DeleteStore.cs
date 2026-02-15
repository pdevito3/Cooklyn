namespace Cooklyn.Server.Domain.Stores.Features;

using Databases;
using MediatR;

public static class DeleteStore
{
    public sealed record Command(string Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var store = await dbContext.Stores.GetById(request.Id, cancellationToken);

            dbContext.Stores.Remove(store);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
