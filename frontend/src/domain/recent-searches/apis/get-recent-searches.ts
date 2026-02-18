import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { RecentSearchDto } from '../types'
import { RecentSearchKeys } from './recent-search.keys'

export async function getRecentSearches(
  pageSize: number = 5,
): Promise<RecentSearchDto[]> {
  const response = await apiClient.get<RecentSearchDto[]>(
    `/api/v1/recent-searches?pageSize=${pageSize}`,
  )
  return response.data
}

export function useRecentSearches() {
  return useQuery({
    queryKey: RecentSearchKeys.list({ pageSize: 50 }),
    queryFn: () => getRecentSearches(50),
  })
}
