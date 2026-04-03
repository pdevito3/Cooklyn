namespace Cooklyn.Server.Domain.Settings;

public class Setting : BaseEntity
{
    public string Key { get; private set; } = default!;
    public string? Value { get; private set; }

    public static Setting Create(string key, string? value)
    {
        return new Setting
        {
            Key = key,
            Value = value
        };
    }

    public void UpdateValue(string? value)
    {
        Value = value;
    }

    protected Setting() { } // EF Core
}
