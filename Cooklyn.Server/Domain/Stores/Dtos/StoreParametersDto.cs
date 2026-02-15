namespace Cooklyn.Server.Domain.Stores.Dtos;

using Resources;

public sealed class StoreParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
