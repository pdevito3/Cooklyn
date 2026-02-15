namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class AddItemsFromCollection
{
    public sealed record Command(string ShoppingListId, AddItemsFromCollectionDto Dto) : IRequest<ShoppingListDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, ShoppingListDto>
    {
        public async Task<ShoppingListDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var shoppingList = await dbContext.ShoppingLists
                .Include(sl => sl.Items)
                .ThenInclude(i => i.RecipeSources)
                .GetById(request.ShoppingListId, cancellationToken);

            var collection = await dbContext.ItemCollections
                .Include(ic => ic.Items)
                .GetById(request.Dto.ItemCollectionId, cancellationToken);

            var maxSortOrder = shoppingList.Items.Any()
                ? shoppingList.Items.Max(i => i.SortOrder)
                : -1;

            foreach (var collectionItem in collection.Items.OrderBy(i => i.SortOrder))
            {
                var itemName = collectionItem.Name.Trim();

                // Try to merge with existing item
                var existingItem = shoppingList.Items.FirstOrDefault(i =>
                    string.Equals(i.Name.Trim(), itemName, StringComparison.OrdinalIgnoreCase)
                    && !i.IsChecked);

                if (existingItem != null
                    && existingItem.Unit.Value == collectionItem.Unit.Value
                    && existingItem.Quantity.HasValue
                    && collectionItem.Quantity.HasValue)
                {
                    existingItem.MergeQuantity(collectionItem.Quantity.Value, collectionItem.Unit);
                }
                else
                {
                    maxSortOrder++;
                    var newItem = ShoppingListItem.Create(new Models.ShoppingListItemForCreation
                    {
                        ShoppingListId = shoppingList.Id,
                        Name = itemName,
                        Quantity = collectionItem.Quantity,
                        Unit = collectionItem.Unit.Value,
                        StoreSectionId = collectionItem.StoreSectionId,
                        SortOrder = maxSortOrder
                    });
                    shoppingList.AddItem(newItem);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return shoppingList.ToShoppingListDto();
        }
    }
}
