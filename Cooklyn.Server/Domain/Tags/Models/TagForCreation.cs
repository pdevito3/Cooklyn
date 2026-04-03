namespace Cooklyn.Server.Domain.Tags.Models;

public sealed record TagForCreation
{
    public string Name { get; init; } = default!;
}
