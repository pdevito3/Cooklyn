namespace Cooklyn.Server.Domain.Ingredients;

using Ardalis.SmartEnum;

public class UnitOfMeasure : ValueObject
{
    private UnitOfMeasureEnum? _unit;
    public string? CustomUnit { get; private set; }

    public string? Value
    {
        get => IsCustom ? CustomUnit : _unit?.Name;
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
                if (parsed == UnitOfMeasureEnum.Custom)
                {
                    // If explicitly set to "Custom", treat as empty
                    _unit = null;
                    CustomUnit = null;
                }
                else
                {
                    _unit = parsed;
                    CustomUnit = null;
                }
            }
            else
            {
                // Unknown value becomes custom
                _unit = UnitOfMeasureEnum.Custom;
                CustomUnit = value;
            }
        }
    }

    public bool IsCustom => _unit == UnitOfMeasureEnum.Custom && !string.IsNullOrWhiteSpace(CustomUnit);
    public bool IsEmpty => _unit == null && string.IsNullOrWhiteSpace(CustomUnit);

    public UnitOfMeasure(string? value)
    {
        Value = value;
    }

    public static UnitOfMeasure Of(string? value) => new(value);
    public static UnitOfMeasure Empty() => new(null);

    // Common unit factory methods
    public static UnitOfMeasure Teaspoon() => new(UnitOfMeasureEnum.Teaspoon.Name);
    public static UnitOfMeasure Tablespoon() => new(UnitOfMeasureEnum.Tablespoon.Name);
    public static UnitOfMeasure Cup() => new(UnitOfMeasureEnum.Cup.Name);
    public static UnitOfMeasure Ounce() => new(UnitOfMeasureEnum.Ounce.Name);
    public static UnitOfMeasure Pound() => new(UnitOfMeasureEnum.Pound.Name);
    public static UnitOfMeasure Gram() => new(UnitOfMeasureEnum.Gram.Name);
    public static UnitOfMeasure Kilogram() => new(UnitOfMeasureEnum.Kilogram.Name);
    public static UnitOfMeasure Milliliter() => new(UnitOfMeasureEnum.Milliliter.Name);
    public static UnitOfMeasure Liter() => new(UnitOfMeasureEnum.Liter.Name);
    public static UnitOfMeasure Pinch() => new(UnitOfMeasureEnum.Pinch.Name);
    public static UnitOfMeasure Dash() => new(UnitOfMeasureEnum.Dash.Name);
    public static UnitOfMeasure Slice() => new(UnitOfMeasureEnum.Slice.Name);
    public static UnitOfMeasure Piece() => new(UnitOfMeasureEnum.Piece.Name);
    public static UnitOfMeasure Whole() => new(UnitOfMeasureEnum.Whole.Name);
    public static UnitOfMeasure Clove() => new(UnitOfMeasureEnum.Clove.Name);

    private UnitOfMeasure() { } // EF Core

    private sealed class UnitOfMeasureEnum(string name, int value) : SmartEnum<UnitOfMeasureEnum>(name, value)
    {
        public static readonly UnitOfMeasureEnum Teaspoon = new("tsp", 1);
        public static readonly UnitOfMeasureEnum Tablespoon = new("Tbsp", 2);
        public static readonly UnitOfMeasureEnum Cup = new("cup", 3);
        public static readonly UnitOfMeasureEnum Ounce = new("oz", 4);
        public static readonly UnitOfMeasureEnum Pound = new("lb", 5);
        public static readonly UnitOfMeasureEnum Gram = new("g", 6);
        public static readonly UnitOfMeasureEnum Kilogram = new("kg", 7);
        public static readonly UnitOfMeasureEnum Milliliter = new("ml", 8);
        public static readonly UnitOfMeasureEnum Liter = new("L", 9);
        public static readonly UnitOfMeasureEnum Pinch = new("pinch", 10);
        public static readonly UnitOfMeasureEnum Dash = new("dash", 11);
        public static readonly UnitOfMeasureEnum Slice = new("slice", 12);
        public static readonly UnitOfMeasureEnum Piece = new("piece", 13);
        public static readonly UnitOfMeasureEnum Whole = new("whole", 14);
        public static readonly UnitOfMeasureEnum Clove = new("clove", 15);
        public static readonly UnitOfMeasureEnum Custom = new("Custom", 99);
    }
}
