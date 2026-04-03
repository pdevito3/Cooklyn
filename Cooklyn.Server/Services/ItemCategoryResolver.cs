namespace Cooklyn.Server.Services;

using Databases;
using Domain.ItemCategoryMappings;
using Domain.ItemCategoryMappings.Models;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

public interface IItemCategoryResolver
{
    Task<string?> ResolveAsync(string itemName, CancellationToken cancellationToken = default);
    Task UpsertMappingAsync(string itemName, string storeSectionId, CancellationToken cancellationToken = default);
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

    private static string GetCacheKey(string normalizedName) =>
        $"item-category:{normalizedName}";

    public async Task<string?> ResolveAsync(string itemName, CancellationToken cancellationToken = default)
    {
        var normalizedName = ItemNameNormalizer.Normalize(itemName);
        if (string.IsNullOrEmpty(normalizedName))
            return null;

        var cacheKey = GetCacheKey(normalizedName);

        return await cache.GetOrSetAsync(
            cacheKey,
            async ct => await LookupStoreSectionIdAsync(normalizedName, ct),
            CacheOptions,
            cancellationToken);
    }

    public async Task UpsertMappingAsync(string itemName, string storeSectionId, CancellationToken cancellationToken = default)
    {
        var normalizedName = ItemNameNormalizer.Normalize(itemName);
        if (string.IsNullOrEmpty(normalizedName))
            return;

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existing = await dbContext.ItemCategoryMappings
            .Where(m => m.NormalizedName == normalizedName)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
        {
            existing.UpdateSection(storeSectionId, "User");
        }
        else
        {
            var mapping = ItemCategoryMapping.Create(new ItemCategoryMappingForCreation
            {
                NormalizedName = normalizedName,
                StoreSectionId = storeSectionId,
                Source = "User"
            });
            dbContext.ItemCategoryMappings.Add(mapping);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var cacheKey = GetCacheKey(normalizedName);
        await cache.RemoveAsync(cacheKey, token: cancellationToken);
    }

    private async Task<string?> LookupStoreSectionIdAsync(string normalizedName, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 1. DB lookup - check for existing mapping
        var dbMapping = await dbContext.ItemCategoryMappings
            .Where(m => m.NormalizedName == normalizedName)
            .Select(m => m.StoreSectionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (dbMapping != null)
            return dbMapping;

        // 2. Seed full-name match
        if (ItemCategorySeedData.Mappings.TryGetValue(normalizedName, out var sectionName))
        {
            var sectionId = await FindSectionByNameAsync(dbContext, sectionName, cancellationToken);
            if (sectionId != null)
            {
                await PersistSeedMappingAsync(dbContext, normalizedName, sectionId, cancellationToken);
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
                    var sectionId = await FindSectionByNameAsync(dbContext, tokenSectionName, cancellationToken);
                    if (sectionId != null)
                    {
                        await PersistSeedMappingAsync(dbContext, normalizedName, sectionId, cancellationToken);
                        return sectionId;
                    }
                }
            }
        }

        // 4. Placeholder for future LLM categorization
        // TODO: LLM categorization

        return null;
    }

    private static async Task<string?> FindSectionByNameAsync(
        AppDbContext dbContext,
        string sectionName,
        CancellationToken cancellationToken)
    {
        return await dbContext.StoreSections
            .Where(s => EF.Functions.ILike(s.Name, $"%{sectionName}%"))
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static async Task PersistSeedMappingAsync(
        AppDbContext dbContext,
        string normalizedName,
        string storeSectionId,
        CancellationToken cancellationToken)
    {
        // Check if mapping already exists (race condition guard)
        var exists = await dbContext.ItemCategoryMappings
            .AnyAsync(m => m.NormalizedName == normalizedName, cancellationToken);

        if (exists)
            return;

        var mapping = ItemCategoryMapping.Create(new ItemCategoryMappingForCreation
        {
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
