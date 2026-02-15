namespace Cooklyn.Server.Domain.Stores.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdateStoreAisles
{
    public sealed record Command(string Id, IReadOnlyList<StoreAisleForUpdateDto> Aisles) : IRequest<StoreDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, StoreDto>
    {
        public async Task<StoreDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var store = await dbContext.Stores
                .Include(s => s.StoreAisles)
                .GetById(request.Id, cancellationToken);

            var existing = store.StoreAisles.OrderBy(a => a.SortOrder).ToList();
            var incoming = request.Aisles;

            // Match by position: update existing, add new, remove extras
            for (var i = 0; i < incoming.Count; i++)
            {
                var dto = incoming[i];
                if (i < existing.Count)
                {
                    existing[i].Update(dto.SortOrder, dto.CustomName);
                    // Update StoreSectionId by removing and re-adding if changed
                    if (existing[i].StoreSectionId != dto.StoreSectionId)
                    {
                        dbContext.StoreAisles.Remove(existing[i]);
                        var newAisle = StoreAisle.Create(store.Id, dto.StoreSectionId, dto.SortOrder, dto.CustomName);
                        await dbContext.StoreAisles.AddAsync(newAisle, cancellationToken);
                    }
                }
                else
                {
                    var aisle = StoreAisle.Create(store.Id, dto.StoreSectionId, dto.SortOrder, dto.CustomName);
                    await dbContext.StoreAisles.AddAsync(aisle, cancellationToken);
                }
            }

            // Delete extras beyond new list length
            if (existing.Count > incoming.Count)
            {
                dbContext.StoreAisles.RemoveRange(existing.Skip(incoming.Count));
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            // Reload
            var loadedStore = await dbContext.Stores
                .AsNoTracking()
                .Include(s => s.StoreAisles)
                .FirstAsync(s => s.Id == store.Id, cancellationToken);

            return loadedStore.ToStoreDto();
        }
    }
}
