namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record UpdateRecipeFlagsDto
{
    public IReadOnlyList<string> Flags { get; init; } = [];
}
