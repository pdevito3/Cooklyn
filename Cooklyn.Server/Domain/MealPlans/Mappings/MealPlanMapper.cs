namespace Cooklyn.Server.Domain.MealPlans.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class MealPlanMapper
{
    [MapperIgnoreSource(nameof(MealPlanEntry.TenantId))]
    [MapperIgnoreSource(nameof(MealPlanEntry.CreatedBy))]
    [MapperIgnoreSource(nameof(MealPlanEntry.LastModifiedBy))]
    [MapperIgnoreSource(nameof(MealPlanEntry.CreatedOn))]
    [MapperIgnoreSource(nameof(MealPlanEntry.LastModifiedOn))]
    [MapperIgnoreSource(nameof(MealPlanEntry.IsDeleted))]
    [MapperIgnoreSource(nameof(MealPlanEntry.DomainEvents))]
    [MapperIgnoreTarget(nameof(MealPlanEntryDto.ImageUrl))]
    public static partial MealPlanEntryDto ToMealPlanEntryDto(this MealPlanEntry entry);

    public static MealPlanEntryDto ToMealPlanEntryDto(this MealPlanEntry entry, string? imageUrl)
    {
        var dto = entry.ToMealPlanEntryDto();
        return dto with { ImageUrl = imageUrl };
    }

    [MapperIgnoreSource(nameof(MealPlanQueue.TenantId))]
    [MapperIgnoreSource(nameof(MealPlanQueue.CreatedBy))]
    [MapperIgnoreSource(nameof(MealPlanQueue.LastModifiedBy))]
    [MapperIgnoreSource(nameof(MealPlanQueue.CreatedOn))]
    [MapperIgnoreSource(nameof(MealPlanQueue.LastModifiedOn))]
    [MapperIgnoreSource(nameof(MealPlanQueue.IsDeleted))]
    [MapperIgnoreSource(nameof(MealPlanQueue.DomainEvents))]
    public static partial MealPlanQueueDto ToMealPlanQueueDto(this MealPlanQueue queue);

    [MapperIgnoreSource(nameof(MealPlanQueueItem.QueueId))]
    [MapperIgnoreSource(nameof(MealPlanQueueItem.CreatedBy))]
    [MapperIgnoreSource(nameof(MealPlanQueueItem.LastModifiedBy))]
    [MapperIgnoreSource(nameof(MealPlanQueueItem.CreatedOn))]
    [MapperIgnoreSource(nameof(MealPlanQueueItem.LastModifiedOn))]
    [MapperIgnoreSource(nameof(MealPlanQueueItem.IsDeleted))]
    [MapperIgnoreSource(nameof(MealPlanQueueItem.DomainEvents))]
    [MapperIgnoreTarget(nameof(MealPlanQueueItemDto.ImageUrl))]
    public static partial MealPlanQueueItemDto ToMealPlanQueueItemDto(this MealPlanQueueItem item);

    public static MealPlanQueueItemDto ToMealPlanQueueItemDto(this MealPlanQueueItem item, string? imageUrl)
    {
        var dto = item.ToMealPlanQueueItemDto();
        return dto with { ImageUrl = imageUrl };
    }

    public static MealPlanEntryForCreation ToMealPlanEntryForCreation(this MealPlanEntryForCreationDto dto, string tenantId)
    {
        return new MealPlanEntryForCreation
        {
            TenantId = tenantId,
            Date = dto.Date,
            EntryType = dto.EntryType,
            RecipeId = dto.RecipeId,
            Title = dto.Title,
            Scale = dto.Scale,
            SortOrder = dto.SortOrder
        };
    }

    public static MealPlanEntryForUpdate ToMealPlanEntryForUpdate(this MealPlanEntryForUpdateDto dto)
    {
        return new MealPlanEntryForUpdate
        {
            Title = dto.Title,
            Scale = dto.Scale,
            SortOrder = dto.SortOrder
        };
    }

    public static MealPlanQueueForCreation ToMealPlanQueueForCreation(this MealPlanQueueForCreationDto dto, string tenantId)
    {
        return new MealPlanQueueForCreation
        {
            TenantId = tenantId,
            Name = dto.Name,
            IsDefault = false
        };
    }

    public static MealPlanQueueForUpdate ToMealPlanQueueForUpdate(this MealPlanQueueForUpdateDto dto)
    {
        return new MealPlanQueueForUpdate
        {
            Name = dto.Name
        };
    }

    public static MealPlanQueueItemForCreation ToMealPlanQueueItemForCreation(this MealPlanQueueItemForCreationDto dto, string queueId, int sortOrder)
    {
        return new MealPlanQueueItemForCreation
        {
            QueueId = queueId,
            RecipeId = dto.RecipeId,
            Title = dto.Title,
            Scale = dto.Scale,
            SortOrder = sortOrder
        };
    }

    private static string MapEntryType(MealPlanEntryType entryType) => entryType.Value;
}
