namespace Cooklyn.Server.Domain.ItemCollections.Features;

using Databases;
using MediatR;

public static class DeleteItemCollection
{
    public sealed record Command(string Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var collection = await dbContext.ItemCollections.GetById(request.Id, cancellationToken);

            dbContext.ItemCollections.Remove(collection);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
