import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type {
  ShoppingListDto,
  ShoppingListForCreationDto,
  ShoppingListForUpdateDto,
  ShoppingListItemDto,
  ShoppingListItemForCreationDto,
  ShoppingListItemForUpdateDto,
  AddItemsFromRecipeDto,
  AddItemsFromCollectionDto,
} from '../types'
import { ShoppingListKeys } from './shopping-list.keys'

export async function createShoppingList(dto: ShoppingListForCreationDto): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>('/api/v1/shoppinglists', dto)
  return response.data
}

export function useCreateShoppingList() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: createShoppingList,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
    },
  })
}

export async function updateShoppingList(id: string, dto: ShoppingListForUpdateDto): Promise<ShoppingListDto> {
  const response = await apiClient.put<ShoppingListDto>(`/api/v1/shoppinglists/${id}`, dto)
  return response.data
}

export function useUpdateShoppingList() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: ShoppingListForUpdateDto }) =>
      updateShoppingList(id, dto),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(id) })
    },
  })
}

export async function deleteShoppingList(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/shoppinglists/${id}`)
}

export function useDeleteShoppingList() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: deleteShoppingList,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
    },
  })
}

export async function completeShoppingList(id: string): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>(`/api/v1/shoppinglists/${id}/complete`)
  return response.data
}

export function useCompleteShoppingList() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: completeShoppingList,
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(id) })
    },
  })
}

export async function reopenShoppingList(id: string): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>(`/api/v1/shoppinglists/${id}/reopen`)
  return response.data
}

export function useReopenShoppingList() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: reopenShoppingList,
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(id) })
    },
  })
}

export async function addShoppingListItem(
  shoppingListId: string,
  dto: ShoppingListItemForCreationDto
): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>(
    `/api/v1/shoppinglists/${shoppingListId}/items`,
    dto
  )
  return response.data
}

export function useAddShoppingListItem() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ shoppingListId, dto }: { shoppingListId: string; dto: ShoppingListItemForCreationDto }) =>
      addShoppingListItem(shoppingListId, dto),
    onSuccess: (_, { shoppingListId }) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(shoppingListId) })
    },
  })
}

export async function updateShoppingListItem(
  shoppingListId: string,
  itemId: string,
  dto: ShoppingListItemForUpdateDto
): Promise<ShoppingListItemDto> {
  const response = await apiClient.put<ShoppingListItemDto>(
    `/api/v1/shoppinglists/${shoppingListId}/items/${itemId}`,
    dto
  )
  return response.data
}

export function useUpdateShoppingListItem() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({
      shoppingListId,
      itemId,
      dto,
    }: {
      shoppingListId: string
      itemId: string
      dto: ShoppingListItemForUpdateDto
    }) => updateShoppingListItem(shoppingListId, itemId, dto),
    onSuccess: (_, { shoppingListId }) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(shoppingListId) })
    },
  })
}

export async function deleteShoppingListItem(
  shoppingListId: string,
  itemId: string
): Promise<void> {
  await apiClient.delete(`/api/v1/shoppinglists/${shoppingListId}/items/${itemId}`)
}

export function useDeleteShoppingListItem() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ shoppingListId, itemId }: { shoppingListId: string; itemId: string }) =>
      deleteShoppingListItem(shoppingListId, itemId),
    onSuccess: (_, { shoppingListId }) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(shoppingListId) })
    },
  })
}

export async function toggleShoppingListItemCheck(
  shoppingListId: string,
  itemId: string
): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>(
    `/api/v1/shoppinglists/${shoppingListId}/items/${itemId}/toggle-check`
  )
  return response.data
}

export function useToggleShoppingListItemCheck() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ shoppingListId, itemId }: { shoppingListId: string; itemId: string }) =>
      toggleShoppingListItemCheck(shoppingListId, itemId),
    onSuccess: (_, { shoppingListId }) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(shoppingListId) })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
    },
  })
}

export async function removeCheckedItems(shoppingListId: string): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>(
    `/api/v1/shoppinglists/${shoppingListId}/remove-checked`
  )
  return response.data
}

export function useRemoveCheckedItems() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: removeCheckedItems,
    onSuccess: (_, shoppingListId) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(shoppingListId) })
    },
  })
}

export async function addItemsFromRecipe(
  shoppingListId: string,
  dto: AddItemsFromRecipeDto
): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>(
    `/api/v1/shoppinglists/${shoppingListId}/add-from-recipe`,
    dto
  )
  return response.data
}

export function useAddItemsFromRecipe() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ shoppingListId, dto }: { shoppingListId: string; dto: AddItemsFromRecipeDto }) =>
      addItemsFromRecipe(shoppingListId, dto),
    onSuccess: (_, { shoppingListId }) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(shoppingListId) })
    },
  })
}

export async function addItemsFromCollection(
  shoppingListId: string,
  dto: AddItemsFromCollectionDto
): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>(
    `/api/v1/shoppinglists/${shoppingListId}/add-from-collection`,
    dto
  )
  return response.data
}

export function useAddItemsFromCollection() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ shoppingListId, dto }: { shoppingListId: string; dto: AddItemsFromCollectionDto }) =>
      addItemsFromCollection(shoppingListId, dto),
    onSuccess: (_, { shoppingListId }) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(shoppingListId) })
    },
  })
}

export function useAddMultipleShoppingListItems() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: async ({
      shoppingListId,
      items,
    }: {
      shoppingListId: string
      items: ShoppingListItemForCreationDto[]
    }) => {
      for (const dto of items) {
        await addShoppingListItem(shoppingListId, dto)
      }
    },
    onSuccess: (_, { shoppingListId }) => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.detail(shoppingListId) })
    },
  })
}
