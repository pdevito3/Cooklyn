namespace Cooklyn.Server.Domain.Tags.Dtos;

using Resources;

public sealed class TagParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
