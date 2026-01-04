namespace Cooklyn.Server.Domain.Tags.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class TagMapper
{
    [MapperIgnoreSource(nameof(Tag.CreatedBy))]
    [MapperIgnoreSource(nameof(Tag.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Tag.CreatedOn))]
    [MapperIgnoreSource(nameof(Tag.LastModifiedOn))]
    [MapperIgnoreSource(nameof(Tag.IsDeleted))]
    [MapperIgnoreSource(nameof(Tag.DomainEvents))]
    public static partial TagDto ToTagDto(this Tag tag);

    public static partial IQueryable<TagDto> ToTagDtoQueryable(this IQueryable<Tag> queryable);

    public static TagForCreation ToTagForCreation(this TagForCreationDto dto, Guid tenantId)
    {
        return new TagForCreation
        {
            TenantId = tenantId,
            Name = dto.Name
        };
    }

    public static partial TagForUpdate ToTagForUpdate(this TagForUpdateDto dto);
}
