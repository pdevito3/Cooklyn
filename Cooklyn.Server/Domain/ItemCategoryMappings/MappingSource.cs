namespace Cooklyn.Server.Domain.ItemCategoryMappings;

using Ardalis.SmartEnum;
using Exceptions;

public partial class MappingSource : ValueObject
{
    private MappingSourceEnum _source = null!;

    public string Value
    {
        get => _source.Name;
        private set
        {
            if (!MappingSourceEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException(nameof(MappingSource), $"Invalid mapping source: {value}");

            _source = parsed;
        }
    }

    public MappingSource(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(MappingSource), "Mapping source cannot be null or empty.");

        Value = value;
    }

    public static MappingSource Of(string value) => new(value);
    public static implicit operator string(MappingSource value) => value.Value;

    public static MappingSource Seed() => new(MappingSourceEnum.Seed.Name);
    public static MappingSource User() => new(MappingSourceEnum.User.Name);
    public static MappingSource System() => new(MappingSourceEnum.System.Name);

    protected MappingSource() { } // EF Core

    private abstract class MappingSourceEnum(string name, int value) : SmartEnum<MappingSourceEnum>(name, value)
    {
        public static readonly MappingSourceEnum Seed = new SeedType();
        public static readonly MappingSourceEnum User = new UserType();
        public static readonly MappingSourceEnum System = new SystemType();

        private class SeedType() : MappingSourceEnum("Seed", 0);
        private class UserType() : MappingSourceEnum("User", 1);
        private class SystemType() : MappingSourceEnum("System", 2);
    }
}
