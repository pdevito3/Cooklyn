namespace Cooklyn.IntegrationTests.FeatureTests.ShoppingLists;

using Cooklyn.Server.Databases;
using Cooklyn.Server.Domain.ItemCategoryMappings;
using Cooklyn.Server.Domain.Recipes;
using Cooklyn.Server.Domain.Recipes.Models;
using Cooklyn.Server.Domain.ShoppingLists;
using Cooklyn.Server.Domain.ShoppingLists.Dtos;
using Cooklyn.Server.Domain.ShoppingLists.Features;
using Cooklyn.Server.Domain.ShoppingLists.Models;
using Cooklyn.Server.Domain.StoreSections;
using Cooklyn.Server.Domain.StoreSections.Models;
using Cooklyn.Server.Resources;
using Cooklyn.Server.Services;
using Microsoft.EntityFrameworkCore;
using Shouldly;

public class AutoCategorizeTests : TestBase
{
    [Fact]
    public async Task add_item_auto_resolves_section_from_seed_data()
    {
        // Arrange
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var produceSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Produce"
        });
        await scope.InsertAsync(produceSection);

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List"
        });
        await scope.InsertAsync(shoppingList);

        // Act - add "Fresh Organic Tomatoes" (should normalize to "tomato" -> Produce)
        var dto = new ShoppingListItemForCreationDto
        {
            Name = "Fresh Organic Tomatoes",
            Quantity = 3
        };
        var command = new AddShoppingListItem.Command(shoppingList.Id, dto);
        var result = await scope.SendAsync(command);

        // Assert
        var addedItem = result.Items.First();
        addedItem.StoreSectionId.ShouldBe(produceSection.Id);
    }

    [Fact]
    public async Task add_item_preserves_explicit_section()
    {
        // Arrange
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var bakingSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Baking"
        });
        await scope.InsertAsync(bakingSection);

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List"
        });
        await scope.InsertAsync(shoppingList);

        // Act - add item with explicit section (should NOT override)
        var dto = new ShoppingListItemForCreationDto
        {
            Name = "Tomatoes",
            StoreSectionId = bakingSection.Id
        };
        var command = new AddShoppingListItem.Command(shoppingList.Id, dto);
        var result = await scope.SendAsync(command);

        // Assert - should keep the explicit section, not resolve to Produce
        var addedItem = result.Items.First();
        addedItem.StoreSectionId.ShouldBe(bakingSection.Id);
    }

    [Fact]
    public async Task add_item_returns_null_section_for_unknown_item()
    {
        // Arrange
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List"
        });
        await scope.InsertAsync(shoppingList);

        // Act - "xanthan gum" is not in seed data
        var dto = new ShoppingListItemForCreationDto { Name = "Xanthan Gum" };
        var command = new AddShoppingListItem.Command(shoppingList.Id, dto);
        var result = await scope.SendAsync(command);

        // Assert
        var addedItem = result.Items.First();
        addedItem.StoreSectionId.ShouldBeNull();
    }

    [Fact]
    public async Task add_item_resolves_via_token_matching()
    {
        // Arrange - "chicken breast" is a multi-word seed entry for "Meat & Seafood"
        // but also test token fallback: "smoked chicken" not in seed -> token "chicken" -> Meat & Seafood
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var meatSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Meat & Seafood"
        });
        await scope.InsertAsync(meatSection);

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List"
        });
        await scope.InsertAsync(shoppingList);

        // Act - "smoked chicken" is not a seed entry, but token "chicken" is
        // Note: "smoked" is not a qualifier so it stays; normalized = "smoked chicken"
        // Full-name "smoked chicken" not in seed -> token "smoked" not in seed -> token "chicken" matches
        var dto = new ShoppingListItemForCreationDto { Name = "Smoked Chicken" };
        var command = new AddShoppingListItem.Command(shoppingList.Id, dto);
        var result = await scope.SendAsync(command);

        // Assert
        var addedItem = result.Items.First();
        addedItem.StoreSectionId.ShouldBe(meatSection.Id);
    }

    [Fact]
    public async Task add_item_resolves_section_with_contains_matching()
    {
        // Arrange - tenant section "Dairy & Eggs" should match seed section "Dairy & Eggs"
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var dairySection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Dairy & Eggs"
        });
        await scope.InsertAsync(dairySection);

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List"
        });
        await scope.InsertAsync(shoppingList);

        // Act
        var dto = new ShoppingListItemForCreationDto { Name = "Eggs" };
        var command = new AddShoppingListItem.Command(shoppingList.Id, dto);
        var result = await scope.SendAsync(command);

        // Assert
        var addedItem = result.Items.First();
        addedItem.StoreSectionId.ShouldBe(dairySection.Id);
    }

    [Fact]
    public async Task add_items_from_recipe_auto_categorizes_new_items()
    {
        // Arrange
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var bakingSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Baking"
        });
        var dairySection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Dairy & Eggs"
        });
        await scope.InsertAsync(bakingSection, dairySection);

        var recipe = Recipe.Create(new RecipeForCreation
        {
            TenantId = tenant.Id,
            Title = "Test Cake"
        });
        await scope.InsertAsync(recipe);

        // Add ingredients to the recipe
        var flour = Ingredient.Create(new IngredientForCreation
        {
            RecipeId = recipe.Id,
            RawText = "2 cups flour",
            Name = "flour",
            Amount = 2,
            Unit = "cup",
            SortOrder = 0
        });
        var eggs = Ingredient.Create(new IngredientForCreation
        {
            RecipeId = recipe.Id,
            RawText = "3 eggs",
            Name = "eggs",
            Amount = 3,
            SortOrder = 1
        });
        var vanilla = Ingredient.Create(new IngredientForCreation
        {
            RecipeId = recipe.Id,
            RawText = "1 tsp vanilla extract",
            Name = "vanilla extract",
            Amount = 1,
            Unit = "tsp",
            SortOrder = 2
        });

        await scope.ExecuteDbContextAsync(async db =>
        {
            var dbRecipe = await db.Recipes.Include(r => r.Ingredients).GetById(recipe.Id);
            dbRecipe.SetIngredients([flour, eggs, vanilla]);
            return await db.SaveChangesAsync();
        });

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Cake Shopping"
        });
        await scope.InsertAsync(shoppingList);

        // Act
        var dto = new AddItemsFromRecipeDto { RecipeId = recipe.Id };
        var command = new AddItemsFromRecipe.Command(shoppingList.Id, dto);
        var result = await scope.SendAsync(command);

        // Assert
        result.Items.Count.ShouldBe(3);

        var flourItem = result.Items.First(i => i.Name == "flour");
        flourItem.StoreSectionId.ShouldBe(bakingSection.Id);

        var eggsItem = result.Items.First(i => i.Name == "eggs");
        eggsItem.StoreSectionId.ShouldBe(dairySection.Id);

        var vanillaItem = result.Items.First(i => i.Name == "vanilla extract");
        vanillaItem.StoreSectionId.ShouldBe(bakingSection.Id);
    }

    [Fact]
    public async Task update_item_section_creates_user_mapping()
    {
        // Arrange
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var produceSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Produce"
        });
        var snacksSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Snacks"
        });
        await scope.InsertAsync(produceSection, snacksSection);

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List"
        });
        await scope.InsertAsync(shoppingList);

        // Add an item (auto-categorized to Produce)
        var addDto = new ShoppingListItemForCreationDto { Name = "Apple" };
        var addResult = await scope.SendAsync(new AddShoppingListItem.Command(shoppingList.Id, addDto));
        var itemId = addResult.Items.First().Id;
        addResult.Items.First().StoreSectionId.ShouldBe(produceSection.Id);

        // Act - user changes section to Snacks
        var updateDto = new ShoppingListItemForUpdateDto
        {
            Name = "Apple",
            StoreSectionId = snacksSection.Id
        };
        var updateCommand = new UpdateShoppingListItem.Command(shoppingList.Id, itemId, updateDto);
        await scope.SendAsync(updateCommand);

        // Assert - a User mapping should have been created/updated
        var mapping = await scope.ExecuteDbContextAsync(async db =>
            await db.ItemCategoryMappings
                .IgnoreQueryFilters([QueryFilterNames.Tenant])
                .FirstOrDefaultAsync(m => m.TenantId == tenant.Id && m.NormalizedName == "apple"));

        mapping.ShouldNotBeNull();
        mapping.StoreSectionId.ShouldBe(snacksSection.Id);
        mapping.Source.Value.ShouldBe("User");
    }

    [Fact]
    public async Task user_mapping_overrides_seed_on_next_add()
    {
        // Arrange
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var produceSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Produce"
        });
        var snacksSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Snacks"
        });
        await scope.InsertAsync(produceSection, snacksSection);

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List"
        });
        await scope.InsertAsync(shoppingList);

        // First add - auto-categorized to Produce
        var addResult1 = await scope.SendAsync(
            new AddShoppingListItem.Command(shoppingList.Id, new ShoppingListItemForCreationDto { Name = "Banana" }));
        var itemId = addResult1.Items.First().Id;
        addResult1.Items.First().StoreSectionId.ShouldBe(produceSection.Id);

        // User correction - move to Snacks
        await scope.SendAsync(new UpdateShoppingListItem.Command(shoppingList.Id, itemId,
            new ShoppingListItemForUpdateDto { Name = "Banana", StoreSectionId = snacksSection.Id }));

        // Act - add "Banana" again on a new list
        var shoppingList2 = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List 2"
        });
        await scope.InsertAsync(shoppingList2);

        // Need to clear cache so we pick up the DB mapping
        var resolver = scope.GetService<IItemCategoryResolver>();
        // The upsert already invalidated cache, but since we're in the same scope,
        // let's add the item and verify it gets the user mapping
        var addResult2 = await scope.SendAsync(
            new AddShoppingListItem.Command(shoppingList2.Id, new ShoppingListItemForCreationDto { Name = "Banana" }));

        // Assert - should use the user's corrected section (Snacks), not the seed (Produce)
        addResult2.Items.First().StoreSectionId.ShouldBe(snacksSection.Id);
    }

    [Fact]
    public async Task seed_mapping_is_persisted_to_db_for_future_lookups()
    {
        // Arrange
        var scope = new TestingServiceScope();
        var tenant = await scope.SetupTenantAsync();

        var spiceSection = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant.Id,
            Name = "Spices & Seasonings"
        });
        await scope.InsertAsync(spiceSection);

        var shoppingList = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant.Id,
            Name = "Test List"
        });
        await scope.InsertAsync(shoppingList);

        // Act - add "cumin" (seed data maps to "Spices & Seasonings")
        var dto = new ShoppingListItemForCreationDto { Name = "Cumin" };
        await scope.SendAsync(new AddShoppingListItem.Command(shoppingList.Id, dto));

        // Assert - a Seed mapping should have been persisted
        var mapping = await scope.ExecuteDbContextAsync(async db =>
            await db.ItemCategoryMappings
                .IgnoreQueryFilters([QueryFilterNames.Tenant])
                .FirstOrDefaultAsync(m => m.TenantId == tenant.Id && m.NormalizedName == "cumin"));

        mapping.ShouldNotBeNull();
        mapping.StoreSectionId.ShouldBe(spiceSection.Id);
        mapping.Source.Value.ShouldBe("Seed");
    }

    [Fact]
    public async Task tenant_isolation_prevents_cross_tenant_mapping_leak()
    {
        // Arrange - two tenants with different sections
        var scope1 = new TestingServiceScope();
        var tenant1 = await scope1.SetupTenantAsync("Tenant A");

        var section1 = StoreSection.Create(new StoreSectionForCreation
        {
            TenantId = tenant1.Id,
            Name = "Produce"
        });
        await scope1.InsertAsync(section1);

        var list1 = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant1.Id,
            Name = "List A"
        });
        await scope1.InsertAsync(list1);

        // Tenant 1 categorizes "garlic"
        await scope1.SendAsync(new AddShoppingListItem.Command(list1.Id,
            new ShoppingListItemForCreationDto { Name = "Garlic" }));

        // Tenant 2 has NO Produce section
        var scope2 = new TestingServiceScope();
        var tenant2 = await scope2.SetupTenantAsync("Tenant B");

        var list2 = ShoppingList.Create(new ShoppingListForCreation
        {
            TenantId = tenant2.Id,
            Name = "List B"
        });
        await scope2.InsertAsync(list2);

        // Act - tenant 2 adds "garlic" but has no matching section
        var result = await scope2.SendAsync(new AddShoppingListItem.Command(list2.Id,
            new ShoppingListItemForCreationDto { Name = "Garlic" }));

        // Assert - should be null since tenant 2 has no Produce section
        result.Items.First().StoreSectionId.ShouldBeNull();
    }
}
