namespace Cooklyn.Server.Databases;

using Domain;
using Domain.ItemCategoryMappings;
using Domain.ItemCollections;
using Domain.Recipes;
using Domain.RecentSearches;
using Domain.MealPlans;
using Domain.SavedFilters;
using Domain.Settings;
using Domain.ShoppingLists;
using Domain.StoreSections;
using Domain.Stores;
using Domain.Tags;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    TimeProvider timeProvider,
    IMediator mediator) : DbContext(options)
{
    #region DbSet Region
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeTag> RecipeTags => Set<RecipeTag>();
    public DbSet<RecipeFlagEntry> RecipeFlagEntries => Set<RecipeFlagEntry>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<NutritionInfo> NutritionInfos => Set<NutritionInfo>();

    public DbSet<StoreSection> StoreSections => Set<StoreSection>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreAisle> StoreAisles => Set<StoreAisle>();

    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<ShoppingListItemRecipeSource> ShoppingListItemRecipeSources => Set<ShoppingListItemRecipeSource>();

    public DbSet<ItemCollection> ItemCollections => Set<ItemCollection>();
    public DbSet<ItemCollectionItem> ItemCollectionItems => Set<ItemCollectionItem>();
    public DbSet<StoreDefaultCollection> StoreDefaultCollections => Set<StoreDefaultCollection>();

    public DbSet<ItemCategoryMapping> ItemCategoryMappings => Set<ItemCategoryMapping>();

    public DbSet<MealPlanEntry> MealPlanEntries => Set<MealPlanEntry>();
    public DbSet<MealPlanQueue> MealPlanQueues => Set<MealPlanQueue>();
    public DbSet<MealPlanQueueItem> MealPlanQueueItems => Set<MealPlanQueueItem>();

    public DbSet<SavedFilter> SavedFilters => Set<SavedFilter>();
    public DbSet<RecentSearch> RecentSearches => Set<RecentSearch>();

    public DbSet<Setting> Settings => Set<Setting>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("cooklyn");

        // Automatically discovers and applies all IEntityTypeConfiguration<T> implementations in the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.FilterSoftDeletedRecords();
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        var result = base.SaveChanges();
        DispatchDomainEvents().GetAwaiter().GetResult();
        return result;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditFields();
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        DispatchDomainEvents().GetAwaiter().GetResult();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEvents();
        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await DispatchDomainEvents();
        return result;
    }

    private async Task DispatchDomainEvents()
    {
        var domainEventEntities = ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var domainEvent in events)
                await mediator.Publish(domainEvent);
        }
    }

    private void UpdateAuditFields()
    {
        var now = timeProvider.GetUtcNow();
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.UpdateCreationProperties(now);
                    entry.Entity.UpdateModifiedProperties(now);
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdateModifiedProperties(now);
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.UpdateModifiedProperties(now);
                    entry.Entity.UpdateIsDeleted(true);
                    break;
            }
        }
    }
}
