import type { IngredientForCreationDto } from '@/domain/recipes/types'
import type { ItemCollectionItemForCreationDto } from '../types'

export function ingredientsToCollectionItems(
  ingredients: IngredientForCreationDto[],
  startSortOrder = 0,
): ItemCollectionItemForCreationDto[] {
  return ingredients.map((ingredient, index) => ({
    name: ingredient.name ?? ingredient.rawText,
    quantity: ingredient.amount ?? null,
    unit: ingredient.unit ?? null,
    storeSectionId: null,
    sortOrder: startSortOrder + index,
  }))
}
