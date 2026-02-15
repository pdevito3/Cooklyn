import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { StoreSectionDto, StoreSectionForCreationDto, StoreSectionForUpdateDto } from '../types'
import { StoreSectionKeys } from './store-section.keys'

export async function createStoreSection(dto: StoreSectionForCreationDto): Promise<StoreSectionDto> {
  const response = await apiClient.post<StoreSectionDto>('/api/v1/storesections', dto)
  return response.data
}

export function useCreateStoreSection() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: createStoreSection,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: StoreSectionKeys.lists() })
    },
  })
}

export async function updateStoreSection(id: string, dto: StoreSectionForUpdateDto): Promise<StoreSectionDto> {
  const response = await apiClient.put<StoreSectionDto>(`/api/v1/storesections/${id}`, dto)
  return response.data
}

export function useUpdateStoreSection() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: StoreSectionForUpdateDto }) =>
      updateStoreSection(id, dto),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: StoreSectionKeys.lists() })
      queryClient.invalidateQueries({ queryKey: StoreSectionKeys.detail(id) })
    },
  })
}

export async function deleteStoreSection(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/storesections/${id}`)
}

export function useDeleteStoreSection() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: deleteStoreSection,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: StoreSectionKeys.lists() })
    },
  })
}
