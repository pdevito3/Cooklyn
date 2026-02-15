import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { StoreDto, StoreForCreationDto, StoreForUpdateDto, StoreAisleForUpdateDto } from '../types'
import { StoreKeys } from './store.keys'

export async function createStore(dto: StoreForCreationDto): Promise<StoreDto> {
  const response = await apiClient.post<StoreDto>('/api/v1/stores', dto)
  return response.data
}

export function useCreateStore() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: createStore,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: StoreKeys.lists() })
    },
  })
}

export async function updateStore(id: string, dto: StoreForUpdateDto): Promise<StoreDto> {
  const response = await apiClient.put<StoreDto>(`/api/v1/stores/${id}`, dto)
  return response.data
}

export function useUpdateStore() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: StoreForUpdateDto }) =>
      updateStore(id, dto),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: StoreKeys.lists() })
      queryClient.invalidateQueries({ queryKey: StoreKeys.detail(id) })
    },
  })
}

export async function deleteStore(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/stores/${id}`)
}

export function useDeleteStore() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: deleteStore,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: StoreKeys.lists() })
    },
  })
}

export async function updateStoreAisles(id: string, aisles: StoreAisleForUpdateDto[]): Promise<StoreDto> {
  const response = await apiClient.put<StoreDto>(`/api/v1/stores/${id}/aisles`, aisles)
  return response.data
}

export function useUpdateStoreAisles() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, aisles }: { id: string; aisles: StoreAisleForUpdateDto[] }) =>
      updateStoreAisles(id, aisles),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: StoreKeys.lists() })
      queryClient.invalidateQueries({ queryKey: StoreKeys.detail(id) })
    },
  })
}

export async function updateStoreDefaultCollections(id: string, itemCollectionIds: string[]): Promise<StoreDto> {
  const response = await apiClient.put<StoreDto>(`/api/v1/stores/${id}/default-collections`, { itemCollectionIds })
  return response.data
}

export function useUpdateStoreDefaultCollections() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, itemCollectionIds }: { id: string; itemCollectionIds: string[] }) =>
      updateStoreDefaultCollections(id, itemCollectionIds),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: StoreKeys.lists() })
      queryClient.invalidateQueries({ queryKey: StoreKeys.detail(id) })
    },
  })
}
