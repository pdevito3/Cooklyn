namespace Cooklyn.Server.Domain.ItemCollections.Dtos;

using Resources;

public sealed class ItemCollectionParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
