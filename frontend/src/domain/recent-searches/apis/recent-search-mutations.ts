import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { RecentSearchDto, RecentSearchForCreationDto } from '../types'
import { RecentSearchKeys } from './recent-search.keys'

export async function addRecentSearch(
  dto: RecentSearchForCreationDto,
): Promise<RecentSearchDto> {
  const response = await apiClient.post<RecentSearchDto>(
    '/api/v1/recent-searches',
    dto,
  )
  return response.data
}

export function useAddRecentSearch() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: addRecentSearch,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: RecentSearchKeys.lists() })
    },
  })
}

export async function deleteRecentSearch(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/recent-searches/${id}`)
}

export function useDeleteRecentSearch() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: deleteRecentSearch,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: RecentSearchKeys.lists() })
    },
  })
}

export async function clearRecentSearches(): Promise<void> {
  await apiClient.delete('/api/v1/recent-searches')
}

export function useClearRecentSearches() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: clearRecentSearches,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: RecentSearchKeys.lists() })
    },
  })
}
