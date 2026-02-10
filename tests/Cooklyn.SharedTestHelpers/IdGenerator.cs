namespace Cooklyn.SharedTestHelpers;

public static class IdGenerator
{
    public static string Tenant() => $"tenant_{Guid.NewGuid():N}";
    public static string User() => $"usr_{Guid.NewGuid():N}";
    public static string UserPermission() => $"usr-perm_{Guid.NewGuid():N}";
    public static string Recipe() => $"rec_{Guid.NewGuid():N}";
    public static string Tag() => $"tag_{Guid.NewGuid():N}";
    public static string RecipeTag() => $"rtag_{Guid.NewGuid():N}";
    public static string RecipeFlagEntry() => $"rfe_{Guid.NewGuid():N}";
    public static string Ingredient() => $"ing_{Guid.NewGuid():N}";
    public static string NutritionInfo() => $"ntrn-info_{Guid.NewGuid():N}";
}
