namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Domain.Recipes;
using Domain.ShoppingLists;
using Domain.ShoppingLists.Dtos;
using Domain.ShoppingLists.Mappings;
using Domain.ShoppingLists.Models;
using Dtos;
using Exceptions;
using Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GenerateShoppingListFromMealPlan
{
    public sealed record Command(BulkShoppingListFromMealPlanDto Dto) : IRequest<ShoppingListDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        IItemCategoryResolver itemCategoryResolver) : IRequestHandler<Command, ShoppingListDto>
    {
        public async Task<ShoppingListDto> Handle(Command request, CancellationToken cancellationToken)
        {
            // Load recipe entries in date range
            var entries = await dbContext.MealPlanEntries
                .AsNoTracking()
                .Where(e => e.Date >= request.Dto.StartDate
                    && e.Date <= request.Dto.EndDate
                    && e.EntryType.Value == "Recipe"
                    && e.RecipeId != null)
                .ToListAsync(cancellationToken);

            // Filter entries based on ingredient selections or exclusions
            var ingredientSelections = request.Dto.EntryIngredientSelections;
            if (ingredientSelections is { Count: > 0 })
            {
                var selectedEntryIds = ingredientSelections.Select(s => s.EntryId).ToHashSet();
                entries = entries.Where(e => selectedEntryIds.Contains(e.Id)).ToList();
            }
            else if (request.Dto.ExcludedEntryIds?.Count > 0)
            {
                entries = entries.Where(e => !request.Dto.ExcludedEntryIds.Contains(e.Id)).ToList();
            }

            if (entries.Count == 0)
                throw new ValidationException(nameof(ShoppingList), "No recipe entries found in the selected date range.");

            // Load recipes with ingredients
            var recipeIds = entries.Select(e => e.RecipeId!).Distinct().ToList();
            var recipes = await dbContext.Recipes
                .AsNoTracking()
                .Include(r => r.Ingredients)
                .Where(r => recipeIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, cancellationToken);

            // Build per-entry ingredient filter lookup
            var ingredientFilter = ingredientSelections?
                .ToDictionary(s => s.EntryId, s => s.IngredientIds?.ToHashSet());

            // Get or create shopping list
            ShoppingList shoppingList;
            if (!string.IsNullOrEmpty(request.Dto.ShoppingListId))
            {
                shoppingList = await dbContext.ShoppingLists
                    .Include(sl => sl.Items)
                    .ThenInclude(i => i.RecipeSources)
                    .FirstOrDefaultAsync(sl => sl.Id == request.Dto.ShoppingListId, cancellationToken)
                    ?? throw new NotFoundException($"Shopping list {request.Dto.ShoppingListId} not found.");
            }
            else
            {
                var listName = !string.IsNullOrWhiteSpace(request.Dto.NewShoppingListName)
                    ? request.Dto.NewShoppingListName
                    : $"Meal Plan {request.Dto.StartDate:MMM d} - {request.Dto.EndDate:MMM d}";

                shoppingList = ShoppingList.Create(new ShoppingListForCreation
                {
                    Name = listName
                });
                await dbContext.ShoppingLists.AddAsync(shoppingList, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            var maxSortOrder = shoppingList.Items.Any()
                ? shoppingList.Items.Max(i => i.SortOrder)
                : -1;

            // Process each entry's recipe ingredients with scale
            foreach (var entry in entries)
            {
                if (!recipes.TryGetValue(entry.RecipeId!, out var recipe))
                    continue;

                // Filter ingredients if per-entry selections are provided
                var entryIngredients = recipe.Ingredients.AsEnumerable();
                if (ingredientFilter != null
                    && ingredientFilter.TryGetValue(entry.Id, out var allowedIds)
                    && allowedIds != null)
                {
                    entryIngredients = entryIngredients.Where(i => allowedIds.Contains(i.Id));
                }

                foreach (var ingredient in entryIngredients)
                {
                    var displayName = ingredient.Name?.Trim() ?? ingredient.RawText;
                    var scaledQuantity = ingredient.Amount.HasValue
                        ? ingredient.Amount.Value * entry.Scale
                        : (decimal?)null;

                    // Try to merge with existing item
                    var existingItem = shoppingList.Items
                        .FirstOrDefault(i => !i.IsChecked
                            && string.Equals(i.Name, displayName, StringComparison.OrdinalIgnoreCase));

                    if (existingItem != null
                        && scaledQuantity.HasValue
                        && existingItem.Quantity.HasValue
                        && existingItem.Unit.Value == ingredient.Unit.Value)
                    {
                        existingItem.MergeQuantity(scaledQuantity.Value, ingredient.Unit);
                        existingItem.AddRecipeSource(ShoppingListItemRecipeSource.Create(
                            existingItem.Id,
                            recipe.Id,
                            ingredient.Amount,
                            ingredient.Unit.Value));
                    }
                    else
                    {
                        var storeSectionId = await itemCategoryResolver.ResolveAsync(displayName, cancellationToken);

                        var newItem = ShoppingListItem.Create(new ShoppingListItemForCreation
                        {
                            ShoppingListId = shoppingList.Id,
                            Name = displayName,
                            Quantity = scaledQuantity,
                            Unit = ingredient.Unit.Value,
                            StoreSectionId = storeSectionId,
                            SortOrder = ++maxSortOrder
                        });

                        shoppingList.AddItem(newItem);
                        await dbContext.SaveChangesAsync(cancellationToken);

                        newItem.AddRecipeSource(ShoppingListItemRecipeSource.Create(
                            newItem.Id,
                            recipe.Id,
                            ingredient.Amount,
                            ingredient.Unit.Value));
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return shoppingList.ToShoppingListDto();
        }
    }
}
