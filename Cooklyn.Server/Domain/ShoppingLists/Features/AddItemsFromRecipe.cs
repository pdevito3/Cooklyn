namespace Cooklyn.Server.Domain.ShoppingLists.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes;
using Services;

public static class AddItemsFromRecipe
{
    public sealed record Command(string ShoppingListId, AddItemsFromRecipeDto Dto) : IRequest<ShoppingListDto>;

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

            var recipe = await dbContext.Recipes
                .Include(r => r.Ingredients)
                .GetById(request.Dto.RecipeId, cancellationToken);

            // Filter ingredients if subset specified
            var ingredients = recipe.Ingredients.AsEnumerable();
            if (request.Dto.IngredientIds is { Count: > 0 })
            {
                var idSet = request.Dto.IngredientIds.ToHashSet();
                ingredients = ingredients.Where(i => idSet.Contains(i.Id));
            }

            var maxSortOrder = shoppingList.Items.Any()
                ? shoppingList.Items.Max(i => i.SortOrder)
                : -1;

            // Resolve tenant once before the loop
            var tenantId = currentUserService.UserIdentifier != null
                ? await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier, cancellationToken)
                : null;

            foreach (var ingredient in ingredients)
            {
                var ingredientName = ingredient.Name?.Trim();
                if (string.IsNullOrEmpty(ingredientName))
                    ingredientName = ingredient.RawText;

                // Try to merge with existing item
                var existingItem = shoppingList.Items.FirstOrDefault(i =>
                    string.Equals(i.Name.Trim(), ingredientName, StringComparison.OrdinalIgnoreCase)
                    && !i.IsChecked);

                if (existingItem != null
                    && existingItem.Unit.Value == ingredient.Unit.Value
                    && existingItem.Quantity.HasValue
                    && ingredient.Amount.HasValue)
                {
                    // Merge quantities
                    existingItem.MergeQuantity(ingredient.Amount.Value, ingredient.Unit);

                    // Add recipe source tracking
                    var source = ShoppingListItemRecipeSource.Create(
                        existingItem.Id,
                        recipe.Id,
                        ingredient.Amount,
                        ingredient.Unit.Value);
                    existingItem.AddRecipeSource(source);
                }
                else
                {
                    // Resolve store section for new item
                    string? resolvedSectionId = tenantId != null
                        ? await itemCategoryResolver.ResolveAsync(ingredientName, tenantId, cancellationToken)
                        : null;

                    // Add as new item
                    maxSortOrder++;
                    var newItem = ShoppingListItem.Create(new Models.ShoppingListItemForCreation
                    {
                        ShoppingListId = shoppingList.Id,
                        Name = ingredientName,
                        Quantity = ingredient.Amount,
                        Unit = ingredient.Unit.Value,
                        StoreSectionId = resolvedSectionId,
                        SortOrder = maxSortOrder
                    });
                    shoppingList.AddItem(newItem);

                    // We need to save first to get the item ID for the recipe source
                    await dbContext.SaveChangesAsync(cancellationToken);

                    var source = ShoppingListItemRecipeSource.Create(
                        newItem.Id,
                        recipe.Id,
                        ingredient.Amount,
                        ingredient.Unit.Value);
                    newItem.AddRecipeSource(source);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return shoppingList.ToShoppingListDto();
        }
    }
}
