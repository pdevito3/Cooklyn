namespace Cooklyn.Server.Domain.Stores.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdateStoreDefaultCollections
{
    public sealed record Command(string Id, IReadOnlyList<string> ItemCollectionIds) : IRequest<StoreDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, StoreDto>
    {
        public async Task<StoreDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var store = await dbContext.Stores
                .Include(s => s.StoreAisles)
                .Include(s => s.StoreDefaultCollections)
                .GetById(request.Id, cancellationToken);

            // Remove existing default collections
            dbContext.StoreDefaultCollections.RemoveRange(store.StoreDefaultCollections);

            // Add new default collections
            var newCollections = request.ItemCollectionIds
                .Select(collectionId => StoreDefaultCollection.Create(store.Id, collectionId))
                .ToList();

            store.SetDefaultCollections(newCollections);

            await dbContext.SaveChangesAsync(cancellationToken);

            return store.ToStoreDto();
        }
    }
}
