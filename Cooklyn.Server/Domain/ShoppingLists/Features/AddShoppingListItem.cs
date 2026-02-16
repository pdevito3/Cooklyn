namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services;

public static class AddShoppingListItem
{
    public sealed record Command(string ShoppingListId, ShoppingListItemForCreationDto Dto) : IRequest<ShoppingListDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        IItemCategoryResolver itemCategoryResolver,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, ShoppingListDto>
    {
        public async Task<ShoppingListDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var shoppingList = await dbContext.ShoppingLists
                .Include(sl => sl.Items)
                .ThenInclude(i => i.RecipeSources)
                .GetById(request.ShoppingListId, cancellationToken);

            var maxSortOrder = shoppingList.Items.Any()
                ? shoppingList.Items.Max(i => i.SortOrder)
                : -1;

            var forCreation = request.Dto.ToShoppingListItemForCreation(shoppingList.Id, maxSortOrder + 1);

            if (forCreation.StoreSectionId is null)
            {
                var tenantId = currentUserService.UserIdentifier != null
                    ? await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier, cancellationToken)
                    : null;

                if (tenantId != null)
                {
                    var resolvedSectionId = await itemCategoryResolver.ResolveAsync(forCreation.Name, tenantId, cancellationToken);
                    if (resolvedSectionId != null)
                        forCreation = forCreation with { StoreSectionId = resolvedSectionId };
                }
            }

            var item = ShoppingListItem.Create(forCreation);
            shoppingList.AddItem(item);

            await dbContext.SaveChangesAsync(cancellationToken);

            return shoppingList.ToShoppingListDto();
        }
    }
}
