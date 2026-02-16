namespace Cooklyn.Server.Domain.ItemCategoryMappings;

using Exceptions;
using Models;

public class ItemCategoryMapping : BaseEntity, ITenantable
{
    public string TenantId { get; private set; } = default!;
    public string NormalizedName { get; private set; } = default!;
    public string StoreSectionId { get; private set; } = default!;
    public MappingSource Source { get; private set; } = default!;

    public static ItemCategoryMapping Create(ItemCategoryMappingForCreation forCreation)
    {
        var mapping = new ItemCategoryMapping
        {
            TenantId = forCreation.TenantId,
            NormalizedName = forCreation.NormalizedName,
            StoreSectionId = forCreation.StoreSectionId,
            Source = MappingSource.Of(forCreation.Source)
        };

        ValidateMapping(mapping);

        return mapping;
    }

    public void UpdateSection(string storeSectionId, string source)
    {
        ValidationException.ThrowWhenNullOrWhitespace(storeSectionId, "Please provide a store section.");

        StoreSectionId = storeSectionId;
        Source = MappingSource.Of(source);
    }

    private static void ValidateMapping(ItemCategoryMapping mapping)
    {
        ValidationException.ThrowWhenNullOrWhitespace(mapping.TenantId, "Please provide a tenant.");
        ValidationException.ThrowWhenNullOrWhitespace(mapping.NormalizedName, "Please provide a normalized name.");
        ValidationException.ThrowWhenNullOrWhitespace(mapping.StoreSectionId, "Please provide a store section.");
    }

    protected ItemCategoryMapping() { } // EF Core
}
