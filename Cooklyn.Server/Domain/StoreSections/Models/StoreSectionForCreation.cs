namespace Cooklyn.Server.Domain.StoreSections.Models;

public sealed record StoreSectionForCreation
{
    public string Name { get; init; } = default!;
}
