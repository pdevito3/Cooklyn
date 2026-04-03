namespace Cooklyn.SharedTestHelpers;

public static class IdGenerator
{
    public static string Recipe() => $"rec_{Guid.NewGuid():N}";
    public static string Tag() => $"tag_{Guid.NewGuid():N}";
    public static string RecipeTag() => $"rtag_{Guid.NewGuid():N}";
    public static string RecipeFlagEntry() => $"rfe_{Guid.NewGuid():N}";
    public static string Ingredient() => $"ing_{Guid.NewGuid():N}";
    public static string NutritionInfo() => $"ntrn-info_{Guid.NewGuid():N}";
}
