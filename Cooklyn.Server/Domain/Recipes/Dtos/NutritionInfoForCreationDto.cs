namespace Cooklyn.Server.Domain.Recipes.Dtos;

public sealed record NutritionInfoForCreationDto
{
    public decimal? Calories { get; init; }
    public decimal? TotalFatGrams { get; init; }
    public decimal? SaturatedFatGrams { get; init; }
    public decimal? TransFatGrams { get; init; }
    public decimal? CholesterolMilligrams { get; init; }
    public decimal? SodiumMilligrams { get; init; }
    public decimal? TotalCarbohydratesGrams { get; init; }
    public decimal? DietaryFiberGrams { get; init; }
    public decimal? TotalSugarsGrams { get; init; }
    public decimal? AddedSugarsGrams { get; init; }
    public decimal? ProteinGrams { get; init; }
    public decimal? VitaminDPercent { get; init; }
    public decimal? CalciumPercent { get; init; }
    public decimal? IronPercent { get; init; }
    public decimal? PotassiumPercent { get; init; }
    public bool IsManuallyEntered { get; init; } = true;
}
