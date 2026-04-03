namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Services;
using Microsoft.EntityFrameworkCore;

public static class UpdateShoppingListItem
{
    public sealed record Command(string ShoppingListId, string ItemId, ShoppingListItemForUpdateDto Dto) : IRequest<ShoppingListDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        IItemCategoryResolver itemCategoryResolver) : IRequestHandler<Command, ShoppingListDto>
    {
        public async Task<ShoppingListDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var shoppingList = await dbContext.ShoppingLists
                .Include(sl => sl.Items)
                .ThenInclude(i => i.RecipeSources)
                .GetById(request.ShoppingListId, cancellationToken);

            var item = shoppingList.Items.FirstOrDefault(i => i.Id == request.ItemId)
                ?? throw new Exceptions.NotFoundException($"{nameof(ShoppingListItem)} not found: {request.ItemId}");

            var oldStoreSectionId = item.StoreSectionId;

            var forUpdate = request.Dto.ToShoppingListItemForUpdate(item.SortOrder);
            item.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            // If user changed the store section, upsert the mapping for future auto-categorization
            if (item.StoreSectionId != null && item.StoreSectionId != oldStoreSectionId)
            {
                await itemCategoryResolver.UpsertMappingAsync(item.Name, item.StoreSectionId, cancellationToken);
            }

            return shoppingList.ToShoppingListDto();
        }
    }
}
