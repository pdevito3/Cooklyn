namespace Cooklyn.Server.Domain.ShoppingLists.Dtos;

using Resources;

public sealed class ShoppingListParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
