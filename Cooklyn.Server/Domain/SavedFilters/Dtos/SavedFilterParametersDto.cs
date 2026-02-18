namespace Cooklyn.Server.Domain.SavedFilters.Dtos;

using Resources;

public sealed class SavedFilterParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
    public string Context { get; set; } = default!;
}
