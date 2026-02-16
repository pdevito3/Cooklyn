export interface ShoppingListItemRecipeSourceDto {
  id: string
  recipeId: string
  originalQuantity: number | null
  originalUnit: string | null
}

export interface ShoppingListItemDto {
  id: string
  shoppingListId: string
  name: string
  quantity: number | null
  unit: string | null
  storeSectionId: string | null
  isChecked: boolean
  checkedOn: string | null
  notes: string | null
  sortOrder: number
  recipeSources: ShoppingListItemRecipeSourceDto[]
}

export interface ShoppingListDto {
  id: string
  tenantId: string
  name: string
  storeId: string | null
  status: string
  completedOn: string | null
  items: ShoppingListItemDto[]
}

export interface ShoppingListSummaryDto {
  id: string
  name: string
  storeId: string | null
  status: string
  itemCount: number
  checkedCount: number
  completedOn: string | null
  createdOn: string
}

export interface ShoppingListForCreationDto {
  name: string
  storeId: string | null
}

export interface ShoppingListForUpdateDto {
  name: string
  storeId: string | null
}

export interface ShoppingListItemForCreationDto {
  name: string
  quantity: number | null
  unit: string | null
  storeSectionId: string | null
  notes: string | null
}

export interface ShoppingListItemForUpdateDto {
  name: string
  quantity: number | null
  unit: string | null
  storeSectionId: string | null
  notes: string | null
}

export interface AddItemsFromRecipeDto {
  recipeId: string
  ingredientIds: string[] | null
}

export interface AddItemsFromCollectionDto {
  itemCollectionId: string
}

export interface ShoppingListParametersDto {
  pageNumber?: number
  pageSize?: number
  filters?: string
  sortOrder?: string
}
