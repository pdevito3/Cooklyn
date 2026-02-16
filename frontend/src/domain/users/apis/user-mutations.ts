import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { UpdateUserDefaultStoreDto } from '../types'
import { UserKeys } from './user.keys'

export async function updateMyDefaultStore(
  dto: UpdateUserDefaultStoreDto,
): Promise<string | null> {
  const response = await apiClient.put<string | null>(
    '/api/v1/users/me/default-store',
    dto,
  )
  return response.data
}

export function useUpdateMyDefaultStore() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: updateMyDefaultStore,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: UserKeys.myDefaultStore() })
    },
  })
}
