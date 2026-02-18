namespace Cooklyn.Server.Domain.RecentSearches.Dtos;

public sealed class RecentSearchParametersDto
{
    public int PageSize { get; set; } = 5;

    private const int MaxPageSize = 50;

    public int GetEffectivePageSize() => PageSize > MaxPageSize ? MaxPageSize : PageSize;
}
