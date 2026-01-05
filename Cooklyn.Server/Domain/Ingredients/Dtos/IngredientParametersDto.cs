namespace Cooklyn.Server.Domain.Ingredients.Dtos;

using Resources;

public sealed class IngredientParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
