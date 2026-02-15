namespace Cooklyn.Server.Domain.StoreSections.Models;

public sealed record StoreSectionForUpdate
{
    public string Name { get; init; } = default!;
}
