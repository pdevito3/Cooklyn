import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { ItemCollectionDto } from '../types'
import { ItemCollectionKeys } from './item-collection.keys'

export async function getItemCollection(id: string): Promise<ItemCollectionDto> {
  const response = await apiClient.get<ItemCollectionDto>(`/api/v1/itemcollections/${id}`)
  return response.data
}

export function useItemCollection(id: string) {
  return useQuery({
    queryKey: ItemCollectionKeys.detail(id),
    queryFn: () => getItemCollection(id),
    enabled: !!id,
  })
}
