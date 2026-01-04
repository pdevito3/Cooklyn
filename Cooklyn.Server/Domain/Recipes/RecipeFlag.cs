namespace Cooklyn.Server.Domain.Recipes;

using Ardalis.SmartEnum;
using Exceptions;

public class RecipeFlag : ValueObject
{
    private RecipeFlagEnum _flag = null!;

    public string Value
    {
        get => _flag.Name;
        private set
        {
            if (!RecipeFlagEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException(nameof(RecipeFlag), $"Invalid recipe flag: {value}");

            _flag = parsed;
        }
    }

    public string DisplayName => _flag.DisplayName;
    public string Icon => _flag.Icon;

    public RecipeFlag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(RecipeFlag), "Recipe flag cannot be null or empty.");

        Value = value;
    }

    public static RecipeFlag Of(string value) => new(value);
    public static implicit operator string(RecipeFlag value) => value.Value;
    public static List<string> ListNames() => RecipeFlagEnum.List.Select(x => x.Name).ToList();
    public static List<RecipeFlag> GetAll() => RecipeFlagEnum.List.Select(x => new RecipeFlag(x.Name)).ToList();

    public static RecipeFlag Vegan() => new(RecipeFlagEnum.Vegan.Name);
    public static RecipeFlag Vegetarian() => new(RecipeFlagEnum.Vegetarian.Name);
    public static RecipeFlag GlutenFree() => new(RecipeFlagEnum.GlutenFree.Name);
    public static RecipeFlag DairyFree() => new(RecipeFlagEnum.DairyFree.Name);
    public static RecipeFlag NutFree() => new(RecipeFlagEnum.NutFree.Name);
    public static RecipeFlag HighProtein() => new(RecipeFlagEnum.HighProtein.Name);
    public static RecipeFlag HighFiber() => new(RecipeFlagEnum.HighFiber.Name);
    public static RecipeFlag LowCarb() => new(RecipeFlagEnum.LowCarb.Name);
    public static RecipeFlag LowFat() => new(RecipeFlagEnum.LowFat.Name);
    public static RecipeFlag LowSodium() => new(RecipeFlagEnum.LowSodium.Name);
    public static RecipeFlag Keto() => new(RecipeFlagEnum.Keto.Name);
    public static RecipeFlag Paleo() => new(RecipeFlagEnum.Paleo.Name);
    public static RecipeFlag Whole30() => new(RecipeFlagEnum.Whole30.Name);
    public static RecipeFlag QuickAndEasy() => new(RecipeFlagEnum.QuickAndEasy.Name);
    public static RecipeFlag MealPrep() => new(RecipeFlagEnum.MealPrep.Name);
    public static RecipeFlag FreezerFriendly() => new(RecipeFlagEnum.FreezerFriendly.Name);
    public static RecipeFlag OnePot() => new(RecipeFlagEnum.OnePot.Name);
    public static RecipeFlag KidFriendly() => new(RecipeFlagEnum.KidFriendly.Name);

    protected RecipeFlag() { } // EF Core

    private abstract class RecipeFlagEnum(string name, int value) : SmartEnum<RecipeFlagEnum>(name, value)
    {
        public abstract string DisplayName { get; }
        public abstract string Icon { get; }

        // Dietary restrictions
        public static readonly RecipeFlagEnum Vegan = new VeganType();
        public static readonly RecipeFlagEnum Vegetarian = new VegetarianType();
        public static readonly RecipeFlagEnum GlutenFree = new GlutenFreeType();
        public static readonly RecipeFlagEnum DairyFree = new DairyFreeType();
        public static readonly RecipeFlagEnum NutFree = new NutFreeType();

        // Nutritional focus
        public static readonly RecipeFlagEnum HighProtein = new HighProteinType();
        public static readonly RecipeFlagEnum HighFiber = new HighFiberType();
        public static readonly RecipeFlagEnum LowCarb = new LowCarbType();
        public static readonly RecipeFlagEnum LowFat = new LowFatType();
        public static readonly RecipeFlagEnum LowSodium = new LowSodiumType();

        // Diet types
        public static readonly RecipeFlagEnum Keto = new KetoType();
        public static readonly RecipeFlagEnum Paleo = new PaleoType();
        public static readonly RecipeFlagEnum Whole30 = new Whole30Type();

        // Convenience
        public static readonly RecipeFlagEnum QuickAndEasy = new QuickAndEasyType();
        public static readonly RecipeFlagEnum MealPrep = new MealPrepType();
        public static readonly RecipeFlagEnum FreezerFriendly = new FreezerFriendlyType();
        public static readonly RecipeFlagEnum OnePot = new OnePotType();
        public static readonly RecipeFlagEnum KidFriendly = new KidFriendlyType();

        private class VeganType() : RecipeFlagEnum("Vegan", 1)
        {
            public override string DisplayName => "Vegan";
            public override string Icon => "leaf";
        }

        private class VegetarianType() : RecipeFlagEnum("Vegetarian", 2)
        {
            public override string DisplayName => "Vegetarian";
            public override string Icon => "carrot";
        }

        private class GlutenFreeType() : RecipeFlagEnum("GlutenFree", 3)
        {
            public override string DisplayName => "Gluten Free";
            public override string Icon => "wheat-off";
        }

        private class DairyFreeType() : RecipeFlagEnum("DairyFree", 4)
        {
            public override string DisplayName => "Dairy Free";
            public override string Icon => "milk-off";
        }

        private class NutFreeType() : RecipeFlagEnum("NutFree", 5)
        {
            public override string DisplayName => "Nut Free";
            public override string Icon => "nut-off";
        }

        private class HighProteinType() : RecipeFlagEnum("HighProtein", 10)
        {
            public override string DisplayName => "High Protein";
            public override string Icon => "beef";
        }

        private class HighFiberType() : RecipeFlagEnum("HighFiber", 11)
        {
            public override string DisplayName => "High Fiber";
            public override string Icon => "grain";
        }

        private class LowCarbType() : RecipeFlagEnum("LowCarb", 12)
        {
            public override string DisplayName => "Low Carb";
            public override string Icon => "scale";
        }

        private class LowFatType() : RecipeFlagEnum("LowFat", 13)
        {
            public override string DisplayName => "Low Fat";
            public override string Icon => "heart";
        }

        private class LowSodiumType() : RecipeFlagEnum("LowSodium", 14)
        {
            public override string DisplayName => "Low Sodium";
            public override string Icon => "salt-off";
        }

        private class KetoType() : RecipeFlagEnum("Keto", 20)
        {
            public override string DisplayName => "Keto";
            public override string Icon => "flame";
        }

        private class PaleoType() : RecipeFlagEnum("Paleo", 21)
        {
            public override string DisplayName => "Paleo";
            public override string Icon => "bone";
        }

        private class Whole30Type() : RecipeFlagEnum("Whole30", 22)
        {
            public override string DisplayName => "Whole30";
            public override string Icon => "check-circle";
        }

        private class QuickAndEasyType() : RecipeFlagEnum("QuickAndEasy", 30)
        {
            public override string DisplayName => "Quick & Easy";
            public override string Icon => "clock";
        }

        private class MealPrepType() : RecipeFlagEnum("MealPrep", 31)
        {
            public override string DisplayName => "Meal Prep";
            public override string Icon => "container";
        }

        private class FreezerFriendlyType() : RecipeFlagEnum("FreezerFriendly", 32)
        {
            public override string DisplayName => "Freezer Friendly";
            public override string Icon => "snowflake";
        }

        private class OnePotType() : RecipeFlagEnum("OnePot", 33)
        {
            public override string DisplayName => "One Pot";
            public override string Icon => "pot";
        }

        private class KidFriendlyType() : RecipeFlagEnum("KidFriendly", 34)
        {
            public override string DisplayName => "Kid Friendly";
            public override string Icon => "smile";
        }
    }
}
