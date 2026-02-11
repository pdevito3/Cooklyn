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

export interface IngredientDto {
  id: string
  rawText: string
  name: string | null
  amount: number | null
  amountText: string | null
  unit: string | null
  customUnit: string | null
  groupName: string | null
  sortOrder: number
}

export interface IngredientForCreationDto {
  rawText: string
  name: string | null
  amount: number | null
  amountText: string | null
  unit: string | null
  customUnit: string | null
  groupName: string | null
  sortOrder: number
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
  servings: number | null
  steps: string | null
  notes: string | null
  tags: string[]
  flags: string[]
  ingredients: IngredientDto[]
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
  servings: number | null
  tags: string[]
  flags: string[]
  ingredientCount: number
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
  servings: number | null
  steps: string | null
  notes: string | null
  tagIds: string[]
  flags: string[]
  ingredients: IngredientForCreationDto[]
  nutritionInfo: NutritionInfoForCreationDto | null
  imageUrl: string | null
}

export interface RecipeForUpdateDto {
  title: string
  description: string | null
  imageS3Bucket: string | null
  imageS3Key: string | null
  rating: string | null
  source: string | null
  servings: number | null
  steps: string | null
  notes: string | null
}

export interface RecipeImageDto {
  imageUrl: string | null
  imageS3Bucket: string | null
  imageS3Key: string | null
}

export interface ImportRecipePreviewDto {
  title: string | null
  description: string | null
  source: string | null
  servings: number | null
  steps: string | null
  ingredients: IngredientForCreationDto[]
  images: ImportImageDto[]
}

export interface ImportImageDto {
  url: string
  alt: string | null
}

export interface CmtImportPreviewItemDto {
  index: number
  title: string
  source: string | null
  servings: number | null
  ingredientCount: number
  rating: number | null
  hasImage: boolean
  isDuplicate: boolean
  tags: string[]
}

export interface CmtImportPreviewDto {
  recipes: CmtImportPreviewItemDto[]
  totalCount: number
  duplicateCount: number
}

export interface CmtImportResultDto {
  importedCount: number
  skippedCount: number
  errorCount: number
  errors: string[]
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

/**
 * Known ingredient unit names (matching backend IngredientUnit SmartEnum)
 */
export const INGREDIENT_UNITS = [
  'Cup',
  'Tablespoon',
  'Teaspoon',
  'FluidOunce',
  'Milliliter',
  'Liter',
  'Pint',
  'Quart',
  'Gallon',
  'Ounce',
  'Pound',
  'Gram',
  'Kilogram',
  'Piece',
  'Whole',
  'Slice',
  'Clove',
  'Pinch',
  'Dash',
  'Can',
  'Bunch',
  'Sprig',
  'Stick',
  'Head',
  'Bag',
  'Jar',
  'Package',
] as const

export type IngredientUnit = (typeof INGREDIENT_UNITS)[number]

/**
 * Maps canonical singular unit names to their plural forms.
 */
export const INGREDIENT_UNIT_PLURALS: Record<string, string> = {
  Cup: 'Cups',
  Tablespoon: 'Tablespoons',
  Teaspoon: 'Teaspoons',
  FluidOunce: 'Fluid Ounces',
  Milliliter: 'Milliliters',
  Liter: 'Liters',
  Pint: 'Pints',
  Quart: 'Quarts',
  Gallon: 'Gallons',
  Ounce: 'Ounces',
  Pound: 'Pounds',
  Gram: 'Grams',
  Kilogram: 'Kilograms',
  Piece: 'Pieces',
  Whole: 'Whole',
  Slice: 'Slices',
  Clove: 'Cloves',
  Pinch: 'Pinches',
  Dash: 'Dashes',
  Can: 'Cans',
  Bunch: 'Bunches',
  Sprig: 'Sprigs',
  Stick: 'Sticks',
  Head: 'Heads',
  Bag: 'Bags',
  Jar: 'Jars',
  Package: 'Packages',
}

/**
 * Returns the display name for a unit, pluralized when amount > 1.
 */
export function formatUnit(unit: string, amount: number | null): string {
  if (amount !== null && amount !== 1) {
    return INGREDIENT_UNIT_PLURALS[unit] ?? unit
  }
  return unit
}
