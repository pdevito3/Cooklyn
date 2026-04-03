namespace Cooklyn.Server.Domain.Settings.Dtos;

public sealed record SettingDto
{
    public string Key { get; init; } = default!;
    public string? Value { get; init; }
}
