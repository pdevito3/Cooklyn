namespace Cooklyn.Server.Domain.Recipes;

using Ardalis.SmartEnum;

public class UnitOfMeasure : ValueObject
{
    private UnitOfMeasureEnum? _unit;

    public string Value
    {
        get => _unit?.Name ?? CustomUnit ?? string.Empty;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _unit = null;
                CustomUnit = null;
                return;
            }

            if (UnitOfMeasureEnum.TryFromName(value, true, out var parsed))
            {
                _unit = parsed;
                CustomUnit = null;
            }
            else
            {
                _unit = UnitOfMeasureEnum.Custom;
                CustomUnit = value;
            }
        }
    }

    public string? CustomUnit { get; private set; }

    public bool IsCustom => _unit == UnitOfMeasureEnum.Custom;
    public bool IsEmpty => _unit == null && string.IsNullOrWhiteSpace(CustomUnit);

    public UnitOfMeasure(string? value)
    {
        Value = value ?? string.Empty;
    }

    public static UnitOfMeasure Of(string? value) => new(value);
    public static implicit operator string(UnitOfMeasure value) => value.Value;
    public static List<string> ListNames() => UnitOfMeasureEnum.List
        .Where(x => x != UnitOfMeasureEnum.Custom)
        .Select(x => x.Name)
        .ToList();

    // Common volume units
    public static UnitOfMeasure Teaspoon() => new(UnitOfMeasureEnum.Teaspoon.Name);
    public static UnitOfMeasure Tablespoon() => new(UnitOfMeasureEnum.Tablespoon.Name);
    public static UnitOfMeasure Cup() => new(UnitOfMeasureEnum.Cup.Name);
    public static UnitOfMeasure FluidOunce() => new(UnitOfMeasureEnum.FluidOunce.Name);
    public static UnitOfMeasure Pint() => new(UnitOfMeasureEnum.Pint.Name);
    public static UnitOfMeasure Quart() => new(UnitOfMeasureEnum.Quart.Name);
    public static UnitOfMeasure Gallon() => new(UnitOfMeasureEnum.Gallon.Name);
    public static UnitOfMeasure Milliliter() => new(UnitOfMeasureEnum.Milliliter.Name);
    public static UnitOfMeasure Liter() => new(UnitOfMeasureEnum.Liter.Name);

    // Common weight units
    public static UnitOfMeasure Ounce() => new(UnitOfMeasureEnum.Ounce.Name);
    public static UnitOfMeasure Pound() => new(UnitOfMeasureEnum.Pound.Name);
    public static UnitOfMeasure Gram() => new(UnitOfMeasureEnum.Gram.Name);
    public static UnitOfMeasure Kilogram() => new(UnitOfMeasureEnum.Kilogram.Name);

    // Common count units
    public static UnitOfMeasure Piece() => new(UnitOfMeasureEnum.Piece.Name);
    public static UnitOfMeasure Pinch() => new(UnitOfMeasureEnum.Pinch.Name);
    public static UnitOfMeasure Dash() => new(UnitOfMeasureEnum.Dash.Name);
    public static UnitOfMeasure Clove() => new(UnitOfMeasureEnum.Clove.Name);
    public static UnitOfMeasure Slice() => new(UnitOfMeasureEnum.Slice.Name);
    public static UnitOfMeasure Can() => new(UnitOfMeasureEnum.Can.Name);
    public static UnitOfMeasure Package() => new(UnitOfMeasureEnum.Package.Name);

    // Custom
    public static UnitOfMeasure Custom(string customValue) => new(customValue);
    public static UnitOfMeasure Empty() => new(null);

    protected UnitOfMeasure() { } // EF Core

    private class UnitOfMeasureEnum(string name, int value) : SmartEnum<UnitOfMeasureEnum>(name, value)
    {
        // Volume - US
        public static readonly UnitOfMeasureEnum Teaspoon = new("tsp", 1);
        public static readonly UnitOfMeasureEnum Tablespoon = new("Tbsp", 2);
        public static readonly UnitOfMeasureEnum Cup = new("cup", 3);
        public static readonly UnitOfMeasureEnum FluidOunce = new("fl oz", 4);
        public static readonly UnitOfMeasureEnum Pint = new("pt", 5);
        public static readonly UnitOfMeasureEnum Quart = new("qt", 6);
        public static readonly UnitOfMeasureEnum Gallon = new("gal", 7);

        // Volume - Metric
        public static readonly UnitOfMeasureEnum Milliliter = new("ml", 10);
        public static readonly UnitOfMeasureEnum Liter = new("L", 11);

        // Weight - US
        public static readonly UnitOfMeasureEnum Ounce = new("oz", 20);
        public static readonly UnitOfMeasureEnum Pound = new("lb", 21);

        // Weight - Metric
        public static readonly UnitOfMeasureEnum Gram = new("g", 30);
        public static readonly UnitOfMeasureEnum Kilogram = new("kg", 31);

        // Count/Other
        public static readonly UnitOfMeasureEnum Piece = new("piece", 40);
        public static readonly UnitOfMeasureEnum Pinch = new("pinch", 41);
        public static readonly UnitOfMeasureEnum Dash = new("dash", 42);
        public static readonly UnitOfMeasureEnum Clove = new("clove", 43);
        public static readonly UnitOfMeasureEnum Slice = new("slice", 44);
        public static readonly UnitOfMeasureEnum Can = new("can", 45);
        public static readonly UnitOfMeasureEnum Package = new("package", 46);

        // Custom marker
        public static readonly UnitOfMeasureEnum Custom = new("custom", 100);
    }
}
