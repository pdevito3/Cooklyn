namespace Cooklyn.Server.Domain.Tags.Models;

public sealed record TagForUpdate
{
    public string Name { get; init; } = default!;
}
