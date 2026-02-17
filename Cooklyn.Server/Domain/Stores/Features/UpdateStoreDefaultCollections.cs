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
                    .ThenInclude(sdc => sdc.ItemCollection)
                .GetById(request.Id, cancellationToken);

            // Phase 1: Remove all existing default collections and flush to DB
            dbContext.StoreDefaultCollections.RemoveRange(store.StoreDefaultCollections);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Phase 2: Add new default collections
            var newCollections = request.ItemCollectionIds
                .Select(collectionId => StoreDefaultCollection.Create(store.Id, collectionId))
                .ToList();

            store.SetDefaultCollections(newCollections);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Load ItemCollection nav properties for the DTO
            foreach (var sdc in store.StoreDefaultCollections)
            {
                await dbContext.Entry(sdc).Reference(x => x.ItemCollection).LoadAsync(cancellationToken);
            }

            return store.ToStoreDto();
        }
    }
}
