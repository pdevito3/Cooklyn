namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record ParseIngredientsRequestDto
{
    public string Text { get; init; } = default!;
}
