namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class AddShoppingList
{
    public sealed record Command(ShoppingListForCreationDto Dto) : IRequest<ShoppingListDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, ShoppingListDto>
    {
        public async Task<ShoppingListDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToShoppingListForCreation();
            var shoppingList = ShoppingList.Create(forCreation);

            await dbContext.ShoppingLists.AddAsync(shoppingList, cancellationToken);

            // Auto-add items from store's default collections
            if (request.Dto.StoreId != null)
            {
                var defaultCollections = await dbContext.StoreDefaultCollections
                    .Where(sdc => sdc.StoreId == request.Dto.StoreId)
                    .Include(sdc => sdc.ItemCollection)
                    .ThenInclude(ic => ic.Items)
                    .ToListAsync(cancellationToken);

                var sortOrder = 0;
                foreach (var sdc in defaultCollections)
                {
                    foreach (var collectionItem in sdc.ItemCollection.Items.OrderBy(i => i.SortOrder))
                    {
                        var item = ShoppingListItem.Create(new Models.ShoppingListItemForCreation
                        {
                            ShoppingListId = shoppingList.Id,
                            Name = collectionItem.Name,
                            Quantity = collectionItem.Quantity,
                            Unit = collectionItem.Unit.Value,
                            StoreSectionId = collectionItem.StoreSectionId,
                            SortOrder = sortOrder++
                        });
                        shoppingList.AddItem(item);
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return shoppingList.ToShoppingListDto();
        }
    }
}
