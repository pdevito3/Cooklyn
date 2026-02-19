namespace Cooklyn.Server.Domain.MealPlans;

using Ardalis.SmartEnum;
using Exceptions;

public partial class MealPlanEntryType : ValueObject
{
    private MealPlanEntryTypeEnum _type = null!;

    public string Value
    {
        get => _type.Name;
        private set
        {
            if (!MealPlanEntryTypeEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException(nameof(MealPlanEntryType), $"Invalid meal plan entry type: {value}");

            _type = parsed;
        }
    }

    public MealPlanEntryType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(nameof(MealPlanEntryType), "Meal plan entry type cannot be null or empty.");

        Value = value;
    }

    public static MealPlanEntryType Of(string value) => new(value);
    public static implicit operator string(MealPlanEntryType value) => value.Value;

    public static MealPlanEntryType Recipe() => new(MealPlanEntryTypeEnum.Recipe.Name);
    public static MealPlanEntryType FreeText() => new(MealPlanEntryTypeEnum.FreeText.Name);

    protected MealPlanEntryType() { } // EF Core

    private abstract class MealPlanEntryTypeEnum(string name, int value) : SmartEnum<MealPlanEntryTypeEnum>(name, value)
    {
        public static readonly MealPlanEntryTypeEnum Recipe = new RecipeType();
        public static readonly MealPlanEntryTypeEnum FreeText = new FreeTextType();

        private class RecipeType() : MealPlanEntryTypeEnum("Recipe", 0);
        private class FreeTextType() : MealPlanEntryTypeEnum("FreeText", 1);
    }
}
