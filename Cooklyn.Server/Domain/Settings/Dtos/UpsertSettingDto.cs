namespace Cooklyn.Server.Domain.Settings.Dtos;

public sealed record UpsertSettingDto
{
    public string? Value { get; init; }
}
