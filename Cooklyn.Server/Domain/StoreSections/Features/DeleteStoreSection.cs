namespace Cooklyn.Server.Domain.StoreSections.Features;

using Databases;
using MediatR;

public static class DeleteStoreSection
{
    public sealed record Command(string Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var storeSection = await dbContext.StoreSections.GetById(request.Id, cancellationToken);

            dbContext.StoreSections.Remove(storeSection);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
