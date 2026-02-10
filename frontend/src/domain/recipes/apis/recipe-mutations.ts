import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type {
  RecipeDto,
  RecipeForCreationDto,
  RecipeForUpdateDto,
  RecipeImageDto,
  IngredientForCreationDto,
  ImportRecipePreviewDto,
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
 * Update recipe ingredients
 */
export async function updateRecipeIngredients(
  id: string,
  ingredients: IngredientForCreationDto[]
): Promise<RecipeDto> {
  const response = await apiClient.put<RecipeDto>(
    `/api/v1/recipes/${id}/ingredients`,
    { ingredients }
  )
  return response.data
}

/**
 * Hook for updating recipe ingredients
 */
export function useUpdateRecipeIngredients() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      id,
      ingredients,
    }: {
      id: string
      ingredients: IngredientForCreationDto[]
    }) => updateRecipeIngredients(id, ingredients),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
      queryClient.invalidateQueries({ queryKey: RecipeKeys.detail(id) })
    },
  })
}

/**
 * Parse free-text ingredients into structured data
 */
export async function parseIngredients(
  text: string
): Promise<IngredientForCreationDto[]> {
  const response = await apiClient.post<IngredientForCreationDto[]>(
    '/api/v1/recipes/parse-ingredients',
    { text }
  )
  return response.data
}

/**
 * Hook for parsing ingredients
 */
export function useParseIngredients() {
  return useMutation({
    mutationFn: parseIngredients,
  })
}

/**
 * Import recipe preview from URL
 */
export async function importRecipePreview(
  url: string
): Promise<ImportRecipePreviewDto> {
  const response = await apiClient.post<ImportRecipePreviewDto>(
    '/api/v1/recipes/import/preview',
    { url }
  )
  return response.data
}

/**
 * Hook for importing recipe preview
 */
export function useImportRecipePreview() {
  return useMutation({
    mutationFn: importRecipePreview,
  })
}

/**
 * Upload recipe image from external URL
 */
export async function uploadRecipeImageFromUrl(
  id: string,
  imageUrl: string
): Promise<RecipeImageDto> {
  const response = await apiClient.post<RecipeImageDto>(
    `/api/v1/recipes/${id}/image-from-url`,
    { imageUrl }
  )
  return response.data
}

/**
 * Hook for uploading recipe image from URL
 */
export function useUploadRecipeImageFromUrl() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, imageUrl }: { id: string; imageUrl: string }) =>
      uploadRecipeImageFromUrl(id, imageUrl),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: RecipeKeys.lists() })
      queryClient.invalidateQueries({ queryKey: RecipeKeys.detail(id) })
    },
  })
}

/**
 * Returns a proxy URL for an external image, to avoid CORS canvas tainting.
 */
export function proxyImageUrl(url: string): string {
  return `/api/v1/recipes/proxy-image?url=${encodeURIComponent(url)}`
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
