import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import { useAuth } from '@/domain/auth/apis/get-user'
import type { UserDto } from '../types'
import { UserKeys } from './user.keys'

export async function getUserByIdentifier(identifier: string): Promise<UserDto> {
  const response = await apiClient.get<UserDto>(`/api/v1/users/by-identifier/${identifier}`)
  return response.data
}

export function useUserByIdentifier(identifier: string | null) {
  return useQuery({
    queryKey: UserKeys.byIdentifier(identifier ?? ''),
    queryFn: () => getUserByIdentifier(identifier!),
    enabled: !!identifier,
    retry: false,
  })
}

export function useCurrentUser() {
  const { sub } = useAuth()
  return useUserByIdentifier(sub)
}
