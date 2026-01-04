namespace Cooklyn.Server.Domain.Recipes.Dtos;

using Resources;

public sealed class RecipeParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
