export interface MealPlanEntryDto {
  id: string
  date: string // DateOnly serialized as "YYYY-MM-DD"
  entryType: 'Recipe' | 'FreeText'
  recipeId: string | null
  title: string
  scale: number
  sortOrder: number
  imageUrl: string | null
}

export interface MealPlanEntryForCreationDto {
  date: string
  entryType: 'Recipe' | 'FreeText'
  recipeId: string | null
  title: string
  scale: number
  sortOrder: number
}

export interface MealPlanEntryForUpdateDto {
  title: string
  scale: number
  sortOrder: number
}

export interface MoveMealPlanEntryDto {
  targetDate: string
  sortOrder: number
}

export interface CopyMealPlanEntryDto {
  targetDate: string
  sortOrder: number
}

export interface MealPlanDayDto {
  date: string
  entries: MealPlanEntryDto[]
}

export interface MealPlanQueueItemDto {
  id: string
  recipeId: string | null
  title: string
  scale: number
  sortOrder: number
  imageUrl: string | null
}

export interface MealPlanQueueItemForCreationDto {
  recipeId: string | null
  title: string
  scale: number
}

export interface MealPlanQueueDto {
  id: string
  name: string
  isDefault: boolean
  items: MealPlanQueueItemDto[]
}

export interface MealPlanQueueForCreationDto {
  name: string
}

export interface MealPlanQueueForUpdateDto {
  name: string
}

export interface AddToCalendarFromQueueDto {
  queueItemId: string
  targetDate: string
  sortOrder: number
}

export interface BulkShoppingListFromMealPlanDto {
  startDate: string
  endDate: string
  shoppingListId: string | null
  newShoppingListName: string | null
  excludedEntryIds: string[] | null
  entryIngredientSelections: MealPlanEntryIngredientSelectionDto[] | null
}

export interface MealPlanEntryIngredientSelectionDto {
  entryId: string
  ingredientIds: string[] | null
}

export interface MealPlanRecipeIngredientsDto {
  entryId: string
  recipeId: string
  recipeTitle: string
  date: string
  scale: number
  ingredients: MealPlanIngredientDto[]
}

export interface MealPlanIngredientDto {
  id: string
  name: string | null
  rawText: string
  amount: number | null
  amountText: string | null
  unit: string | null
}
