import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { StoreDto } from '../types'
import { StoreKeys } from './store.keys'

export async function getStore(id: string): Promise<StoreDto> {
  const response = await apiClient.get<StoreDto>(`/api/v1/stores/${id}`)
  return response.data
}

export function useStore(id: string) {
  return useQuery({
    queryKey: StoreKeys.detail(id),
    queryFn: () => getStore(id),
    enabled: !!id,
  })
}
