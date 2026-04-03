namespace Cooklyn.Server.Domain.Recipes.Mappings;

using Dtos;
using Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class NutritionInfoMapper
{
    [MapperIgnoreSource(nameof(NutritionInfo.CreatedOn))]
    [MapperIgnoreSource(nameof(NutritionInfo.LastModifiedOn))]
    [MapperIgnoreSource(nameof(NutritionInfo.IsDeleted))]
    [MapperIgnoreSource(nameof(NutritionInfo.DomainEvents))]
    public static partial NutritionInfoDto ToNutritionInfoDto(this NutritionInfo nutritionInfo);

    public static NutritionInfoForCreation ToNutritionInfoForCreation(
        this NutritionInfoForCreationDto dto,
        string recipeId)
    {
        return new NutritionInfoForCreation
        {
            RecipeId = recipeId,
            Calories = dto.Calories,
            TotalFatGrams = dto.TotalFatGrams,
            SaturatedFatGrams = dto.SaturatedFatGrams,
            TransFatGrams = dto.TransFatGrams,
            CholesterolMilligrams = dto.CholesterolMilligrams,
            SodiumMilligrams = dto.SodiumMilligrams,
            TotalCarbohydratesGrams = dto.TotalCarbohydratesGrams,
            DietaryFiberGrams = dto.DietaryFiberGrams,
            TotalSugarsGrams = dto.TotalSugarsGrams,
            AddedSugarsGrams = dto.AddedSugarsGrams,
            ProteinGrams = dto.ProteinGrams,
            VitaminDPercent = dto.VitaminDPercent,
            CalciumPercent = dto.CalciumPercent,
            IronPercent = dto.IronPercent,
            PotassiumPercent = dto.PotassiumPercent,
            IsManuallyEntered = dto.IsManuallyEntered
        };
    }

    public static partial NutritionInfoForUpdate ToNutritionInfoForUpdate(this NutritionInfoForUpdateDto dto);
}
