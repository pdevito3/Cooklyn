import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type {
  SavedFilterDto,
  SavedFilterForCreationDto,
  SavedFilterForUpdateDto,
} from '../types'
import { SavedFilterKeys } from './saved-filter.keys'

export async function createSavedFilter(
  dto: SavedFilterForCreationDto,
): Promise<SavedFilterDto> {
  const response = await apiClient.post<SavedFilterDto>(
    '/api/v1/saved-filters',
    dto,
  )
  return response.data
}

export function useCreateSavedFilter() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: createSavedFilter,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: SavedFilterKeys.lists() })
    },
  })
}

export async function updateSavedFilter(
  id: string,
  dto: SavedFilterForUpdateDto,
): Promise<SavedFilterDto> {
  const response = await apiClient.put<SavedFilterDto>(
    `/api/v1/saved-filters/${id}`,
    dto,
  )
  return response.data
}

export function useUpdateSavedFilter() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: SavedFilterForUpdateDto }) =>
      updateSavedFilter(id, dto),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: SavedFilterKeys.lists() })
      queryClient.invalidateQueries({ queryKey: SavedFilterKeys.detail(id) })
    },
  })
}

export async function deleteSavedFilter(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/saved-filters/${id}`)
}

export function useDeleteSavedFilter() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: deleteSavedFilter,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: SavedFilterKeys.lists() })
    },
  })
}
