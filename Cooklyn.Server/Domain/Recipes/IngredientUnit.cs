namespace Cooklyn.Server.Domain.Recipes;

using Ardalis.SmartEnum;

public class IngredientUnit : ValueObject
{
    private IngredientUnitEnum? _unit;

    public string Value
    {
        get => _unit?.Name ?? string.Empty;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _unit = null;
                return;
            }

            if (!IngredientUnitEnum.TryFromName(value, true, out var parsed))
            {
                // Unknown unit — treat as Custom
                _unit = IngredientUnitEnum.Custom;
                return;
            }

            _unit = parsed;
        }
    }

    public string Abbreviation => _unit?.Abbreviation ?? string.Empty;
    public string PluralName => _unit?.PluralName ?? string.Empty;
    public bool IsCustom => _unit == IngredientUnitEnum.Custom;

    public IngredientUnit(string value)
    {
        Value = value;
    }

    public static IngredientUnit Of(string value) => new(value);
    public static implicit operator string(IngredientUnit value) => value.Value;
    public static List<string> ListNames() => IngredientUnitEnum.List.Where(x => x != IngredientUnitEnum.Custom).Select(x => x.Name).ToList();
    public static List<IngredientUnit> GetAll() => IngredientUnitEnum.List.Where(x => x != IngredientUnitEnum.Custom).Select(x => new IngredientUnit(x.Name)).ToList();

    /// <summary>
    /// Attempts to parse a text string into a known unit by checking aliases.
    /// Returns null if no match found.
    /// </summary>
    public static IngredientUnit? TryParse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var trimmed = text.Trim();

        // First pass: case-sensitive alias check (needed for T vs t)
        foreach (var unit in IngredientUnitEnum.List)
        {
            if (unit == IngredientUnitEnum.Custom)
                continue;

            if (unit.ParseAliases.Any(a => a.Equals(trimmed, StringComparison.Ordinal)))
                return new IngredientUnit(unit.Name);
        }

        // Second pass: case-insensitive match on Name, Abbreviation, PluralName, and aliases
        foreach (var unit in IngredientUnitEnum.List)
        {
            if (unit == IngredientUnitEnum.Custom)
                continue;

            if (unit.Name.Equals(trimmed, StringComparison.OrdinalIgnoreCase) 
                || unit.Abbreviation.Equals(trimmed, StringComparison.OrdinalIgnoreCase) 
                || unit.PluralName.Equals(trimmed, StringComparison.OrdinalIgnoreCase) 
                || unit.ParseAliases.Any(a => a.Equals(trimmed, StringComparison.OrdinalIgnoreCase)))
                return new IngredientUnit(unit.Name);
        }

        return null;
    }

    protected IngredientUnit() { } // EF Core

    private abstract class IngredientUnitEnum(string name, int value) : SmartEnum<IngredientUnitEnum>(name, value)
    {
        public abstract string Abbreviation { get; }
        public abstract string PluralName { get; }
        public virtual string[] ParseAliases => [];

        // Volume
        public static readonly IngredientUnitEnum Cup = new CupType();
        public static readonly IngredientUnitEnum Tablespoon = new TablespoonType();
        public static readonly IngredientUnitEnum Teaspoon = new TeaspoonType();
        public static readonly IngredientUnitEnum FluidOunce = new FluidOunceType();
        public static readonly IngredientUnitEnum Milliliter = new MilliliterType();
        public static readonly IngredientUnitEnum Liter = new LiterType();
        public static readonly IngredientUnitEnum Pint = new PintType();
        public static readonly IngredientUnitEnum Quart = new QuartType();
        public static readonly IngredientUnitEnum Gallon = new GallonType();

        // Weight
        public static readonly IngredientUnitEnum Ounce = new OunceType();
        public static readonly IngredientUnitEnum Pound = new PoundType();
        public static readonly IngredientUnitEnum Gram = new GramType();
        public static readonly IngredientUnitEnum Kilogram = new KilogramType();

        // Count/Other
        public static readonly IngredientUnitEnum Piece = new PieceType();
        public static readonly IngredientUnitEnum Whole = new WholeType();
        public static readonly IngredientUnitEnum Slice = new SliceType();
        public static readonly IngredientUnitEnum Clove = new CloveType();
        public static readonly IngredientUnitEnum Pinch = new PinchType();
        public static readonly IngredientUnitEnum Dash = new DashType();
        public static readonly IngredientUnitEnum Can = new CanType();
        public static readonly IngredientUnitEnum Bunch = new BunchType();
        public static readonly IngredientUnitEnum Sprig = new SprigType();
        public static readonly IngredientUnitEnum Stick = new StickType();
        public static readonly IngredientUnitEnum Head = new HeadType();
        public static readonly IngredientUnitEnum Bag = new BagType();
        public static readonly IngredientUnitEnum Jar = new JarType();
        public static readonly IngredientUnitEnum Package = new PackageType();

        // Custom sentinel
        public static readonly IngredientUnitEnum Custom = new CustomType();

        private class CupType() : IngredientUnitEnum("Cup", 1)
        {
            public override string Abbreviation => "c";
            public override string PluralName => "Cups";
            public override string[] ParseAliases => ["cup", "cups", "c", "c."];
        }

        private class TablespoonType() : IngredientUnitEnum("Tablespoon", 2)
        {
            public override string Abbreviation => "tbsp";
            public override string PluralName => "Tablespoons";
            public override string[] ParseAliases => ["tbsp", "tbsps", "tbsp.", "tbs", "tbs.", "T", "tablespoon", "tablespoons"];
        }

        private class TeaspoonType() : IngredientUnitEnum("Teaspoon", 3)
        {
            public override string Abbreviation => "tsp";
            public override string PluralName => "Teaspoons";
            public override string[] ParseAliases => ["tsp", "tsps", "tsp.", "t", "teaspoon", "teaspoons"];
        }

        private class FluidOunceType() : IngredientUnitEnum("FluidOunce", 4)
        {
            public override string Abbreviation => "fl oz";
            public override string PluralName => "Fluid Ounces";
            public override string[] ParseAliases => ["fl oz", "fl. oz.", "fluid ounce", "fluid ounces", "floz"];
        }

        private class MilliliterType() : IngredientUnitEnum("Milliliter", 5)
        {
            public override string Abbreviation => "ml";
            public override string PluralName => "Milliliters";
            public override string[] ParseAliases => ["ml", "ml.", "mls", "milliliter", "milliliters", "millilitre", "millilitres"];
        }

        private class LiterType() : IngredientUnitEnum("Liter", 6)
        {
            public override string Abbreviation => "L";
            public override string PluralName => "Liters";
            public override string[] ParseAliases => ["l", "l.", "liter", "liters", "litre", "litres"];
        }

        private class PintType() : IngredientUnitEnum("Pint", 7)
        {
            public override string Abbreviation => "pt";
            public override string PluralName => "Pints";
            public override string[] ParseAliases => ["pt", "pt.", "pint", "pints"];
        }

        private class QuartType() : IngredientUnitEnum("Quart", 8)
        {
            public override string Abbreviation => "qt";
            public override string PluralName => "Quarts";
            public override string[] ParseAliases => ["qt", "qt.", "quart", "quarts"];
        }

        private class GallonType() : IngredientUnitEnum("Gallon", 9)
        {
            public override string Abbreviation => "gal";
            public override string PluralName => "Gallons";
            public override string[] ParseAliases => ["gal", "gal.", "gallon", "gallons"];
        }

        private class OunceType() : IngredientUnitEnum("Ounce", 10)
        {
            public override string Abbreviation => "oz";
            public override string PluralName => "Ounces";
            public override string[] ParseAliases => ["oz", "oz.", "ounce", "ounces"];
        }

        private class PoundType() : IngredientUnitEnum("Pound", 11)
        {
            public override string Abbreviation => "lb";
            public override string PluralName => "Pounds";
            public override string[] ParseAliases => ["lb", "lb.", "lbs", "lbs.", "pound", "pounds"];
        }

        private class GramType() : IngredientUnitEnum("Gram", 12)
        {
            public override string Abbreviation => "g";
            public override string PluralName => "Grams";
            public override string[] ParseAliases => ["g", "g.", "gr", "gram", "grams"];
        }

        private class KilogramType() : IngredientUnitEnum("Kilogram", 13)
        {
            public override string Abbreviation => "kg";
            public override string PluralName => "Kilograms";
            public override string[] ParseAliases => ["kg", "kg.", "kgs", "kilogram", "kilograms"];
        }

        private class PieceType() : IngredientUnitEnum("Piece", 20)
        {
            public override string Abbreviation => "pc";
            public override string PluralName => "Pieces";
            public override string[] ParseAliases => ["pc", "pcs", "piece", "pieces"];
        }

        private class WholeType() : IngredientUnitEnum("Whole", 21)
        {
            public override string Abbreviation => "whole";
            public override string PluralName => "Whole";
            public override string[] ParseAliases => ["whole"];
        }

        private class SliceType() : IngredientUnitEnum("Slice", 22)
        {
            public override string Abbreviation => "slice";
            public override string PluralName => "Slices";
            public override string[] ParseAliases => ["slice", "slices"];
        }

        private class CloveType() : IngredientUnitEnum("Clove", 23)
        {
            public override string Abbreviation => "clove";
            public override string PluralName => "Cloves";
            public override string[] ParseAliases => ["clove", "cloves"];
        }

        private class PinchType() : IngredientUnitEnum("Pinch", 24)
        {
            public override string Abbreviation => "pinch";
            public override string PluralName => "Pinches";
            public override string[] ParseAliases => ["pinch", "pinches"];
        }

        private class DashType() : IngredientUnitEnum("Dash", 25)
        {
            public override string Abbreviation => "dash";
            public override string PluralName => "Dashes";
            public override string[] ParseAliases => ["dash", "dashes"];
        }

        private class CanType() : IngredientUnitEnum("Can", 26)
        {
            public override string Abbreviation => "can";
            public override string PluralName => "Cans";
            public override string[] ParseAliases => ["can", "cans"];
        }

        private class BunchType() : IngredientUnitEnum("Bunch", 27)
        {
            public override string Abbreviation => "bunch";
            public override string PluralName => "Bunches";
            public override string[] ParseAliases => ["bunch", "bunches"];
        }

        private class SprigType() : IngredientUnitEnum("Sprig", 28)
        {
            public override string Abbreviation => "sprig";
            public override string PluralName => "Sprigs";
            public override string[] ParseAliases => ["sprig", "sprigs"];
        }

        private class StickType() : IngredientUnitEnum("Stick", 29)
        {
            public override string Abbreviation => "stick";
            public override string PluralName => "Sticks";
            public override string[] ParseAliases => ["stick", "sticks"];
        }

        private class HeadType() : IngredientUnitEnum("Head", 30)
        {
            public override string Abbreviation => "head";
            public override string PluralName => "Heads";
            public override string[] ParseAliases => ["head", "heads"];
        }

        private class BagType() : IngredientUnitEnum("Bag", 31)
        {
            public override string Abbreviation => "bag";
            public override string PluralName => "Bags";
            public override string[] ParseAliases => ["bag", "bags"];
        }

        private class JarType() : IngredientUnitEnum("Jar", 32)
        {
            public override string Abbreviation => "jar";
            public override string PluralName => "Jars";
            public override string[] ParseAliases => ["jar", "jars"];
        }

        private class PackageType() : IngredientUnitEnum("Package", 33)
        {
            public override string Abbreviation => "pkg";
            public override string PluralName => "Packages";
            public override string[] ParseAliases => ["pkg", "pkg.", "pkgs", "package", "packages"];
        }

        private class CustomType() : IngredientUnitEnum("Custom", 99)
        {
            public override string Abbreviation => "";
            public override string PluralName => "";
        }
    }
}
