import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { ShoppingListDto } from '../types'
import { ShoppingListKeys } from './shopping-list.keys'

export async function getShoppingList(id: string): Promise<ShoppingListDto> {
  const response = await apiClient.get<ShoppingListDto>(`/api/v1/shoppinglists/${id}`)
  return response.data
}

export function useShoppingList(id: string) {
  return useQuery({
    queryKey: ShoppingListKeys.detail(id),
    queryFn: () => getShoppingList(id),
    enabled: !!id,
  })
}
