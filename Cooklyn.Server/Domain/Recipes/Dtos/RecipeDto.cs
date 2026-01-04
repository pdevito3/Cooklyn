namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record RecipeDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public string Rating { get; init; } = default!;
    public string? Source { get; init; }
    public bool IsFavorite { get; init; }
    public int? Servings { get; init; }
    public string? Steps { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<RecipeIngredientDto> Ingredients { get; init; } = [];
    public IReadOnlyList<string> Tags { get; init; } = [];
    public IReadOnlyList<string> Flags { get; init; } = [];
    public NutritionInfoDto? NutritionInfo { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset? LastModifiedOn { get; init; }
}
