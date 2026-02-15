import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import { UserKeys } from './user.keys'

export async function getMyDefaultStore(): Promise<string | null> {
  const response = await apiClient.get<string | null>('/api/v1/users/me/default-store')
  return response.data
}

export function useMyDefaultStore() {
  return useQuery({
    queryKey: UserKeys.myDefaultStore(),
    queryFn: getMyDefaultStore,
    retry: false,
  })
}
