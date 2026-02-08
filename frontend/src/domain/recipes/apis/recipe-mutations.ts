import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type {
  RecipeDto,
  RecipeForCreationDto,
  RecipeForUpdateDto,
  RecipeImageDto,
} from '../types'
import { RecipeKeys } from './recipe.keys'

/**
 * Create a new recipe
 */
export async function createRecipe(dto: RecipeForCreationDto): Promise<RecipeDto> {
  const response = await apiClient.post<RecipeDto>('/api/v1/recipes', dto)
  return response.data
}

/**
 * Hook for creating a recipe
 */
export function useCreateRecipe() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: createRecipe,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
    },
  })
}

/**
 * Update an existing recipe
 */
export async function updateRecipe(
  id: string,
  dto: RecipeForUpdateDto
): Promise<RecipeDto> {
  const response = await apiClient.put<RecipeDto>(`/api/v1/recipes/${id}`, dto)
  return response.data
}

/**
 * Hook for updating a recipe
 */
export function useUpdateRecipe() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: RecipeForUpdateDto }) =>
      updateRecipe(id, dto),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
      queryClient.invalidateQueries({ queryKey: RecipeKeys.detail(id) })
    },
  })
}

/**
 * Delete a recipe
 */
export async function deleteRecipe(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/recipes/${id}`)
}

/**
 * Hook for deleting a recipe
 */
export function useDeleteRecipe() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: deleteRecipe,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
    },
  })
}

/**
 * Toggle favorite status of a recipe
 */
export async function toggleRecipeFavorite(id: string): Promise<RecipeDto> {
  const response = await apiClient.post<RecipeDto>(
    `/api/v1/recipes/${id}/toggle-favorite`
  )
  return response.data
}

/**
 * Hook for toggling recipe favorite status
 */
export function useToggleRecipeFavorite() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: toggleRecipeFavorite,
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
      queryClient.invalidateQueries({ queryKey: RecipeKeys.detail(id) })
    },
  })
}

/**
 * Upload an image for a recipe
 */
export async function uploadRecipeImage(
  id: string,
  file: File
): Promise<RecipeImageDto> {
  const formData = new FormData()
  formData.append('file', file)

  const response = await apiClient.post<RecipeImageDto>(
    `/api/v1/recipes/${id}/image`,
    formData,
    {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    }
  )
  return response.data
}

/**
 * Hook for uploading a recipe image
 */
export function useUploadRecipeImage() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, file }: { id: string; file: File }) =>
      uploadRecipeImage(id, file),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
      queryClient.invalidateQueries({ queryKey: RecipeKeys.detail(id) })
    },
  })
}

/**
 * Delete the image of a recipe
 */
export async function deleteRecipeImage(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/recipes/${id}/image`)
}

/**
 * Hook for deleting a recipe image
 */
export function useDeleteRecipeImage() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: deleteRecipeImage,
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
      queryClient.invalidateQueries({ queryKey: RecipeKeys.detail(id) })
    },
  })
}

/**
 * Update recipe rating
 */
export async function updateRecipeRating(
  id: string,
  rating: string
): Promise<RecipeDto> {
  const response = await apiClient.put<RecipeDto>(
    `/api/v1/recipes/${id}/rating`,
    { rating }
  )
  return response.data
}

/**
 * Hook for updating recipe rating
 */
export function useUpdateRecipeRating() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, rating }: { id: string; rating: string }) =>
      updateRecipeRating(id, rating),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
      queryClient.invalidateQueries({ queryKey: RecipeKeys.detail(id) })
    },
  })
}
