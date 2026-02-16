namespace Cooklyn.Server.Services;

using Databases;
using Domain.ItemCategoryMappings;
using Domain.ItemCategoryMappings.Models;
using Microsoft.EntityFrameworkCore;
using Resources;
using ZiggyCreatures.Caching.Fusion;

public interface IItemCategoryResolver
{
    Task<string?> ResolveAsync(string itemName, string tenantId, CancellationToken cancellationToken = default);
    Task UpsertMappingAsync(string itemName, string storeSectionId, string tenantId, CancellationToken cancellationToken = default);
}

public sealed class ItemCategoryResolver(
    IFusionCache cache,
    IServiceScopeFactory scopeFactory) : IItemCategoryResolver
{
    private static readonly FusionCacheEntryOptions CacheOptions = new()
    {
        Duration = TimeSpan.FromMinutes(30),
        IsFailSafeEnabled = true,
        FailSafeMaxDuration = TimeSpan.FromHours(2),
        FailSafeThrottleDuration = TimeSpan.FromSeconds(30)
    };

    private static string GetCacheKey(string tenantId, string normalizedName) =>
        $"item-category:{tenantId}:{normalizedName}";

    public async Task<string?> ResolveAsync(string itemName, string tenantId, CancellationToken cancellationToken = default)
    {
        var normalizedName = ItemNameNormalizer.Normalize(itemName);
        if (string.IsNullOrEmpty(normalizedName))
            return null;

        var cacheKey = GetCacheKey(tenantId, normalizedName);

        return await cache.GetOrSetAsync(
            cacheKey,
            async ct => await LookupStoreSectionIdAsync(normalizedName, tenantId, ct),
            CacheOptions,
            cancellationToken);
    }

    public async Task UpsertMappingAsync(string itemName, string storeSectionId, string tenantId, CancellationToken cancellationToken = default)
    {
        var normalizedName = ItemNameNormalizer.Normalize(itemName);
        if (string.IsNullOrEmpty(normalizedName))
            return;

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existing = await dbContext.ItemCategoryMappings
            .IgnoreQueryFilters([QueryFilterNames.Tenant])
            .Where(m => m.TenantId == tenantId && m.NormalizedName == normalizedName)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
        {
            existing.UpdateSection(storeSectionId, "User");
        }
        else
        {
            var mapping = ItemCategoryMapping.Create(new ItemCategoryMappingForCreation
            {
                TenantId = tenantId,
                NormalizedName = normalizedName,
                StoreSectionId = storeSectionId,
                Source = "User"
            });
            dbContext.ItemCategoryMappings.Add(mapping);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var cacheKey = GetCacheKey(tenantId, normalizedName);
        await cache.RemoveAsync(cacheKey, token: cancellationToken);
    }

    private async Task<string?> LookupStoreSectionIdAsync(string normalizedName, string tenantId, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 1. DB lookup - check for existing tenant-specific mapping
        var dbMapping = await dbContext.ItemCategoryMappings
            .IgnoreQueryFilters([QueryFilterNames.Tenant])
            .Where(m => m.TenantId == tenantId && m.NormalizedName == normalizedName)
            .Select(m => m.StoreSectionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (dbMapping != null)
            return dbMapping;

        // 2. Seed full-name match
        if (ItemCategorySeedData.Mappings.TryGetValue(normalizedName, out var sectionName))
        {
            var sectionId = await FindTenantSectionByNameAsync(dbContext, tenantId, sectionName, cancellationToken);
            if (sectionId != null)
            {
                await PersistSeedMappingAsync(dbContext, tenantId, normalizedName, sectionId, cancellationToken);
                return sectionId;
            }
        }

        // 3. Seed token match (only if multi-token)
        var tokens = ItemNameNormalizer.ExtractTokens(normalizedName);
        if (tokens.Length > 1)
        {
            // Iterate tokens left-to-right for best match
            foreach (var token in tokens)
            {
                if (ItemCategorySeedData.Mappings.TryGetValue(token, out var tokenSectionName))
                {
                    var sectionId = await FindTenantSectionByNameAsync(dbContext, tenantId, tokenSectionName, cancellationToken);
                    if (sectionId != null)
                    {
                        await PersistSeedMappingAsync(dbContext, tenantId, normalizedName, sectionId, cancellationToken);
                        return sectionId;
                    }
                }
            }
        }

        // 4. Placeholder for future LLM categorization
        // TODO: LLM categorization

        return null;
    }

    private static async Task<string?> FindTenantSectionByNameAsync(
        AppDbContext dbContext,
        string tenantId,
        string sectionName,
        CancellationToken cancellationToken)
    {
        return await dbContext.StoreSections
            .Where(s => s.TenantId == tenantId && EF.Functions.ILike(s.Name, $"%{sectionName}%"))
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static async Task PersistSeedMappingAsync(
        AppDbContext dbContext,
        string tenantId,
        string normalizedName,
        string storeSectionId,
        CancellationToken cancellationToken)
    {
        // Check if mapping already exists (race condition guard)
        var exists = await dbContext.ItemCategoryMappings
            .IgnoreQueryFilters([QueryFilterNames.Tenant])
            .AnyAsync(m => m.TenantId == tenantId && m.NormalizedName == normalizedName, cancellationToken);

        if (exists)
            return;

        var mapping = ItemCategoryMapping.Create(new ItemCategoryMappingForCreation
        {
            TenantId = tenantId,
            NormalizedName = normalizedName,
            StoreSectionId = storeSectionId,
            Source = "Seed"
        });
        dbContext.ItemCategoryMappings.Add(mapping);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Race condition - another request already persisted this mapping
        }
    }
}
