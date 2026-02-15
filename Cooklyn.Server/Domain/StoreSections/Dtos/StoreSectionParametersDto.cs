namespace Cooklyn.Server.Domain.StoreSections.Dtos;

using Resources;

public sealed class StoreSectionParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
