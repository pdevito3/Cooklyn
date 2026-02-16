import type { IngredientForCreationDto } from '@/domain/recipes/types'
import type { ItemCollectionItemDto } from '../types'

export function collectionItemsToIngredients(
  items: ItemCollectionItemDto[],
): IngredientForCreationDto[] {
  return items.map((item, index) => {
    const parts: string[] = []
    if (item.quantity != null) parts.push(String(item.quantity))
    if (item.unit) parts.push(item.unit)
    parts.push(item.name)

    return {
      rawText: parts.join(' '),
      name: item.name,
      amount: item.quantity,
      amountText: item.quantity != null ? String(item.quantity) : null,
      unit: item.unit,
      customUnit: null,
      groupName: null,
      sortOrder: item.sortOrder ?? index,
    }
  })
}
