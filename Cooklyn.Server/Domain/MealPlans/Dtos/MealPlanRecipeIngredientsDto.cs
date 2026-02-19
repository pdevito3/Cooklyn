namespace Cooklyn.Server.Domain.MealPlans.Dtos;

public sealed record MealPlanRecipeIngredientsDto
{
    public string EntryId { get; init; } = default!;
    public string RecipeId { get; init; } = default!;
    public string RecipeTitle { get; init; } = default!;
    public DateOnly Date { get; init; }
    public decimal Scale { get; init; }
    public IReadOnlyList<MealPlanIngredientDto> Ingredients { get; init; } = [];
}

public sealed record MealPlanIngredientDto
{
    public string Id { get; init; } = default!;
    public string? Name { get; init; }
    public string RawText { get; init; } = default!;
    public decimal? Amount { get; init; }
    public string? AmountText { get; init; }
    public string? Unit { get; init; }
}
