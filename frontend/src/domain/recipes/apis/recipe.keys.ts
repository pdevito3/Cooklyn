/**
 * Query Key Factory for Recipe domain
 *
 * Provides type-safe, hierarchical query keys for TanStack Query.
 */
export const RecipeKeys = {
  all: ['recipes'] as const,
  lists: () => [...RecipeKeys.all, 'list'] as const,
  list: (params?: {
    pageNumber?: number
    pageSize?: number
    filters?: string
    sortOrder?: string
  }) => [...RecipeKeys.lists(), params] as const,
  details: () => [...RecipeKeys.all, 'detail'] as const,
  detail: (id: string) => [...RecipeKeys.details(), id] as const,
}
