namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class DeleteShoppingListItem
{
    public sealed record Command(string ShoppingListId, string ItemId) : IRequest<ShoppingListDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, ShoppingListDto>
    {
        public async Task<ShoppingListDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var shoppingList = await dbContext.ShoppingLists
                .Include(sl => sl.Items)
                .ThenInclude(i => i.RecipeSources)
                .GetById(request.ShoppingListId, cancellationToken);

            var item = shoppingList.Items.FirstOrDefault(i => i.Id == request.ItemId)
                ?? throw new Exceptions.NotFoundException($"{nameof(ShoppingListItem)} not found: {request.ItemId}");

            dbContext.ShoppingListItems.Remove(item);
            await dbContext.SaveChangesAsync(cancellationToken);

            return shoppingList.ToShoppingListDto();
        }
    }
}
