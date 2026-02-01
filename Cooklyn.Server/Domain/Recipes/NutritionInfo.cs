namespace Cooklyn.Server.Domain.Recipes;

using Recipes.Models;

public class NutritionInfo : BaseEntity
{
    public Guid RecipeId { get; private set; }

    // Macronutrients (per serving)
    public decimal? Calories { get; private set; }
    public decimal? TotalFatGrams { get; private set; }
    public decimal? SaturatedFatGrams { get; private set; }
    public decimal? TransFatGrams { get; private set; }
    public decimal? CholesterolMilligrams { get; private set; }
    public decimal? SodiumMilligrams { get; private set; }
    public decimal? TotalCarbohydratesGrams { get; private set; }
    public decimal? DietaryFiberGrams { get; private set; }
    public decimal? TotalSugarsGrams { get; private set; }
    public decimal? AddedSugarsGrams { get; private set; }
    public decimal? ProteinGrams { get; private set; }

    // Micronutrients (percentage of daily value)
    public decimal? VitaminDPercent { get; private set; }
    public decimal? CalciumPercent { get; private set; }
    public decimal? IronPercent { get; private set; }
    public decimal? PotassiumPercent { get; private set; }

    // Indicates if this was manually entered
    public bool IsManuallyEntered { get; private set; }

    public static NutritionInfo Create(NutritionInfoForCreation forCreation)
    {
        var nutritionInfo = new NutritionInfo
        {
            RecipeId = forCreation.RecipeId,
            Calories = forCreation.Calories,
            TotalFatGrams = forCreation.TotalFatGrams,
            SaturatedFatGrams = forCreation.SaturatedFatGrams,
            TransFatGrams = forCreation.TransFatGrams,
            CholesterolMilligrams = forCreation.CholesterolMilligrams,
            SodiumMilligrams = forCreation.SodiumMilligrams,
            TotalCarbohydratesGrams = forCreation.TotalCarbohydratesGrams,
            DietaryFiberGrams = forCreation.DietaryFiberGrams,
            TotalSugarsGrams = forCreation.TotalSugarsGrams,
            AddedSugarsGrams = forCreation.AddedSugarsGrams,
            ProteinGrams = forCreation.ProteinGrams,
            VitaminDPercent = forCreation.VitaminDPercent,
            CalciumPercent = forCreation.CalciumPercent,
            IronPercent = forCreation.IronPercent,
            PotassiumPercent = forCreation.PotassiumPercent,
            IsManuallyEntered = forCreation.IsManuallyEntered
        };

        return nutritionInfo;
    }

    public NutritionInfo Update(NutritionInfoForUpdate forUpdate)
    {
        Calories = forUpdate.Calories;
        TotalFatGrams = forUpdate.TotalFatGrams;
        SaturatedFatGrams = forUpdate.SaturatedFatGrams;
        TransFatGrams = forUpdate.TransFatGrams;
        CholesterolMilligrams = forUpdate.CholesterolMilligrams;
        SodiumMilligrams = forUpdate.SodiumMilligrams;
        TotalCarbohydratesGrams = forUpdate.TotalCarbohydratesGrams;
        DietaryFiberGrams = forUpdate.DietaryFiberGrams;
        TotalSugarsGrams = forUpdate.TotalSugarsGrams;
        AddedSugarsGrams = forUpdate.AddedSugarsGrams;
        ProteinGrams = forUpdate.ProteinGrams;
        VitaminDPercent = forUpdate.VitaminDPercent;
        CalciumPercent = forUpdate.CalciumPercent;
        IronPercent = forUpdate.IronPercent;
        PotassiumPercent = forUpdate.PotassiumPercent;
        IsManuallyEntered = forUpdate.IsManuallyEntered;

        return this;
    }

    public void MarkAsCalculated()
    {
        IsManuallyEntered = false;
    }

    public void MarkAsManual()
    {
        IsManuallyEntered = true;
    }

    protected NutritionInfo() { } // EF Core
}
