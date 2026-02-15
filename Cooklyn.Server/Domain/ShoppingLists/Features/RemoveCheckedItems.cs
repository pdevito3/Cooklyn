namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class RemoveCheckedItems
{
    public sealed record Command(string ShoppingListId) : IRequest<ShoppingListDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, ShoppingListDto>
    {
        public async Task<ShoppingListDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var shoppingList = await dbContext.ShoppingLists
                .Include(sl => sl.Items)
                .ThenInclude(i => i.RecipeSources)
                .GetById(request.ShoppingListId, cancellationToken);

            var checkedItems = shoppingList.Items.Where(i => i.IsChecked).ToList();
            dbContext.ShoppingListItems.RemoveRange(checkedItems);

            await dbContext.SaveChangesAsync(cancellationToken);

            // Reload to get accurate state
            var reloaded = await dbContext.ShoppingLists
                .AsNoTracking()
                .Include(sl => sl.Items)
                .ThenInclude(i => i.RecipeSources)
                .FirstAsync(sl => sl.Id == shoppingList.Id, cancellationToken);

            return reloaded.ToShoppingListDto();
        }
    }
}
