/**
 * Recipe domain types
 */

export interface NutritionInfoDto {
  id: string
  recipeId: string
  calories: number | null
  totalFatGrams: number | null
  saturatedFatGrams: number | null
  transFatGrams: number | null
  cholesterolMilligrams: number | null
  sodiumMilligrams: number | null
  totalCarbohydratesGrams: number | null
  dietaryFiberGrams: number | null
  totalSugarsGrams: number | null
  addedSugarsGrams: number | null
  proteinGrams: number | null
  vitaminDPercent: number | null
  calciumPercent: number | null
  ironPercent: number | null
  potassiumPercent: number | null
  isManuallyEntered: boolean
}

export interface RecipeDto {
  id: string
  tenantId: string
  title: string
  description: string | null
  imageUrl: string | null
  imageS3Bucket: string | null
  imageS3Key: string | null
  rating: string
  source: string | null
  isFavorite: boolean
  servings: number | null
  steps: string | null
  notes: string | null
  tags: string[]
  flags: string[]
  nutritionInfo: NutritionInfoDto | null
  createdOn: string
  lastModifiedOn: string | null
}

export interface RecipeSummaryDto {
  id: string
  title: string
  description: string | null
  imageUrl: string | null
  rating: string
  isFavorite: boolean
  servings: number | null
  tags: string[]
  flags: string[]
  createdOn: string
}

export interface NutritionInfoForCreationDto {
  calories: number | null
  totalFatGrams: number | null
  saturatedFatGrams: number | null
  transFatGrams: number | null
  cholesterolMilligrams: number | null
  sodiumMilligrams: number | null
  totalCarbohydratesGrams: number | null
  dietaryFiberGrams: number | null
  totalSugarsGrams: number | null
  addedSugarsGrams: number | null
  proteinGrams: number | null
  vitaminDPercent: number | null
  calciumPercent: number | null
  ironPercent: number | null
  potassiumPercent: number | null
  isManuallyEntered: boolean
}

export interface RecipeForCreationDto {
  title: string
  description: string | null
  imageS3Bucket: string | null
  imageS3Key: string | null
  rating: string | null
  source: string | null
  isFavorite: boolean
  servings: number | null
  steps: string | null
  notes: string | null
  tagIds: string[]
  flags: string[]
  nutritionInfo: NutritionInfoForCreationDto | null
}

export interface RecipeForUpdateDto {
  title: string
  description: string | null
  imageS3Bucket: string | null
  imageS3Key: string | null
  rating: string | null
  source: string | null
  isFavorite: boolean
  servings: number | null
  steps: string | null
  notes: string | null
}

export interface RecipeImageDto {
  imageUrl: string | null
  imageS3Bucket: string | null
  imageS3Key: string | null
}

export interface PagedList<T> {
  items: T[]
  pageNumber: number
  totalPages: number
  pageSize: number
  totalCount: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface RecipeParametersDto {
  pageNumber?: number
  pageSize?: number
  filters?: string
  sortOrder?: string
}

/**
 * Recipe ratings enum values
 */
export const RECIPE_RATINGS = [
  'Not Rated',
  'Loved It',
  'Liked It',
  'It Was Ok',
  'Not Great',
  'Hated It',
] as const

export type RecipeRating = (typeof RECIPE_RATINGS)[number]

/**
 * Recipe flags enum values
 */
export const RECIPE_FLAGS = [
  'Vegan',
  'Vegetarian',
  'Gluten-Free',
  'Dairy-Free',
  'Nut-Free',
  'High-Protein',
  'High-Fiber',
  'Low-Carb',
  'Low-Fat',
  'Low-Sodium',
  'Keto',
  'Paleo',
  'Whole30',
  'Quick & Easy',
  'Meal Prep',
  'Freezer-Friendly',
  'One-Pot',
  'Kid-Friendly',
] as const

export type RecipeFlag = (typeof RECIPE_FLAGS)[number]
