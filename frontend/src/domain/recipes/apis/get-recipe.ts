import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { RecipeDto } from '../types'
import { RecipeKeys } from './recipe.keys'

/**
 * Fetch a single recipe by ID
 */
export async function getRecipe(id: string): Promise<RecipeDto> {
  const response = await apiClient.get<RecipeDto>(`/api/v1/recipes/${id}`)
  return response.data
}

/**
 * Hook for fetching a single recipe
 */
export function useRecipe(id: string) {
  return useQuery({
    queryKey: RecipeKeys.detail(id),
    queryFn: () => getRecipe(id),
    enabled: !!id,
  })
}
