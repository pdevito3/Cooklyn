import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { ShoppingListSummaryDto, ShoppingListParametersDto } from '../types'
import { ShoppingListKeys } from './shopping-list.keys'
import type { PaginationInfo } from '@/domain/recipes/apis/get-recipes'

export interface ShoppingListListResponse {
  items: ShoppingListSummaryDto[]
  pagination: PaginationInfo
}

export async function getShoppingLists(
  params?: ShoppingListParametersDto
): Promise<ShoppingListListResponse> {
  const queryParams = new URLSearchParams()

  if (params?.pageNumber) queryParams.set('pageNumber', params.pageNumber.toString())
  if (params?.pageSize) queryParams.set('pageSize', params.pageSize.toString())
  if (params?.filters) queryParams.set('filters', params.filters)
  if (params?.sortOrder) queryParams.set('sortOrder', params.sortOrder)

  const queryString = queryParams.toString()
  const url = `/api/v1/shoppinglists${queryString ? `?${queryString}` : ''}`

  const response = await apiClient.get<ShoppingListSummaryDto[]>(url)

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
      // Use default pagination
    }
  }

  return { items: response.data, pagination }
}

export function useShoppingLists(params?: ShoppingListParametersDto) {
  return useQuery({
    queryKey: ShoppingListKeys.list(params),
    queryFn: () => getShoppingLists(params),
  })
}
