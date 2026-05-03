import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import { RecipeKeys } from './recipe.keys'

export async function getRecipeSourceDomains(): Promise<string[]> {
  const response = await apiClient.get<string[]>('/api/v1/recipes/source-domains')
  return response.data
}

export function useRecipeSourceDomains() {
  return useQuery({
    queryKey: RecipeKeys.sourceDomains(),
    queryFn: getRecipeSourceDomains,
  })
}
