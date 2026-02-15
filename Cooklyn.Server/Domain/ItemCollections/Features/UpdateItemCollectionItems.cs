namespace Cooklyn.Server.Domain.ItemCollections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdateItemCollectionItems
{
    public sealed record Command(string Id, IReadOnlyList<ItemCollectionItemForCreationDto> Items) : IRequest<ItemCollectionDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, ItemCollectionDto>
    {
        public async Task<ItemCollectionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var collection = await dbContext.ItemCollections
                .Include(ic => ic.Items)
                .GetById(request.Id, cancellationToken);

            var existing = collection.Items.OrderBy(i => i.SortOrder).ToList();
            var incoming = request.Items;

            // Match by position: update existing, add new, remove extras
            for (var i = 0; i < incoming.Count; i++)
            {
                var dto = incoming[i];
                if (i < existing.Count)
                {
                    existing[i].Update(dto.Name, dto.Quantity, dto.Unit, dto.StoreSectionId, dto.SortOrder);
                }
                else
                {
                    var item = ItemCollectionItem.Create(collection.Id, dto.Name, dto.Quantity, dto.Unit, dto.StoreSectionId, dto.SortOrder);
                    await dbContext.ItemCollectionItems.AddAsync(item, cancellationToken);
                }
            }

            // Delete extras beyond new list length
            if (existing.Count > incoming.Count)
            {
                dbContext.ItemCollectionItems.RemoveRange(existing.Skip(incoming.Count));
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            // Reload
            var loaded = await dbContext.ItemCollections
                .AsNoTracking()
                .Include(ic => ic.Items)
                .FirstAsync(ic => ic.Id == collection.Id, cancellationToken);

            return loaded.ToItemCollectionDto();
        }
    }
}
