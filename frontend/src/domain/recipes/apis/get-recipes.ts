import { useQuery, useInfiniteQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { RecipeSummaryDto, RecipeParametersDto } from '../types'
import { RecipeKeys } from './recipe.keys'

// Pagination info is returned in response headers from the API

export interface PaginationInfo {
  pageNumber: number
  totalPages: number
  pageSize: number
  totalCount: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface RecipeListResponse {
  items: RecipeSummaryDto[]
  pagination: PaginationInfo
}

/**
 * Fetch paginated list of recipes
 */
export async function getRecipes(
  params?: RecipeParametersDto
): Promise<RecipeListResponse> {
  const queryParams = new URLSearchParams()

  if (params?.pageNumber) queryParams.set('pageNumber', params.pageNumber.toString())
  if (params?.pageSize) queryParams.set('pageSize', params.pageSize.toString())
  if (params?.filters) queryParams.set('filters', params.filters)
  if (params?.sortOrder) queryParams.set('sortOrder', params.sortOrder)

  const queryString = queryParams.toString()
  const url = `/api/v1/recipes${queryString ? `?${queryString}` : ''}`

  const response = await apiClient.get<RecipeSummaryDto[]>(url)

  // Parse pagination from headers
  const paginationHeader = response.headers['x-pagination']
  let pagination: PaginationInfo = {
    pageNumber: 1,
    totalPages: 1,
    pageSize: 10,
    totalCount: response.data.length,
    hasPrevious: false,
    hasNext: false,
  }

  if (paginationHeader) {
    try {
      pagination = JSON.parse(paginationHeader)
    } catch {
      // Use default pagination if header is invalid
    }
  }

  return {
    items: response.data,
    pagination,
  }
}

/**
 * Hook for fetching paginated recipe list
 */
export function useRecipes(params?: RecipeParametersDto) {
  return useQuery({
    queryKey: RecipeKeys.list(params),
    queryFn: () => getRecipes(params),
  })
}

/**
 * Hook for infinite scrolling recipe list
 */
export function useInfiniteRecipes(params?: Omit<RecipeParametersDto, 'pageNumber'>) {
  return useInfiniteQuery({
    queryKey: RecipeKeys.list(params),
    queryFn: ({ pageParam }) => getRecipes({ ...params, pageNumber: pageParam }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.pagination.hasNext ? lastPage.pagination.pageNumber + 1 : undefined,
  })
}
