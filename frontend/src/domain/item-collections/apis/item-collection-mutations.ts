import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type {
  ItemCollectionDto,
  ItemCollectionForCreationDto,
  ItemCollectionForUpdateDto,
  ItemCollectionItemForCreationDto,
} from '../types'
import { ItemCollectionKeys } from './item-collection.keys'

export async function createItemCollection(
  dto: ItemCollectionForCreationDto,
): Promise<ItemCollectionDto> {
  const response = await apiClient.post<ItemCollectionDto>(
    '/api/v1/itemcollections',
    dto,
  )
  return response.data
}

export function useCreateItemCollection() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: createItemCollection,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ItemCollectionKeys.lists() })
    },
  })
}

export async function updateItemCollection(
  id: string,
  dto: ItemCollectionForUpdateDto,
): Promise<ItemCollectionDto> {
  const response = await apiClient.put<ItemCollectionDto>(
    `/api/v1/itemcollections/${id}`,
    dto,
  )
  return response.data
}

export function useUpdateItemCollection() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({
      id,
      dto,
    }: {
      id: string
      dto: ItemCollectionForUpdateDto
    }) => updateItemCollection(id, dto),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: ItemCollectionKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ItemCollectionKeys.detail(id) })
    },
  })
}

export async function deleteItemCollection(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/itemcollections/${id}`)
}

export function useDeleteItemCollection() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: deleteItemCollection,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ItemCollectionKeys.lists() })
    },
  })
}

export async function updateItemCollectionItems(
  id: string,
  items: ItemCollectionItemForCreationDto[],
): Promise<ItemCollectionDto> {
  const response = await apiClient.put<ItemCollectionDto>(
    `/api/v1/itemcollections/${id}/items`,
    items,
  )
  return response.data
}

export function useUpdateItemCollectionItems() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({
      id,
      items,
    }: {
      id: string
      items: ItemCollectionItemForCreationDto[]
    }) => updateItemCollectionItems(id, items),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: ItemCollectionKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ItemCollectionKeys.detail(id) })
    },
  })
}
