namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetShoppingList
{
    public sealed record Query(string Id) : IRequest<ShoppingListDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, ShoppingListDto>
    {
        public async Task<ShoppingListDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var shoppingList = await dbContext.ShoppingLists
                .Include(sl => sl.Items)
                .ThenInclude(i => i.RecipeSources)
                .GetById(request.Id, cancellationToken);

            return shoppingList.ToShoppingListDto();
        }
    }
}
