namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using MediatR;

public static class DeleteShoppingList
{
    public sealed record Command(string Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var shoppingList = await dbContext.ShoppingLists.GetById(request.Id, cancellationToken);

            dbContext.ShoppingLists.Remove(shoppingList);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
