import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { SavedFilterDto, SavedFilterParametersDto } from '../types'
import { SavedFilterKeys } from './saved-filter.keys'

export interface PaginationInfo {
  pageNumber: number
  totalPages: number
  pageSize: number
  totalCount: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface SavedFilterListResponse {
  items: SavedFilterDto[]
  pagination: PaginationInfo
}

export async function getSavedFilters(
  params: SavedFilterParametersDto,
): Promise<SavedFilterListResponse> {
  const queryParams = new URLSearchParams()

  queryParams.set('context', params.context)
  if (params.pageNumber)
    queryParams.set('pageNumber', params.pageNumber.toString())
  if (params.pageSize) queryParams.set('pageSize', params.pageSize.toString())
  if (params.filters) queryParams.set('filters', params.filters)
  if (params.sortOrder) queryParams.set('sortOrder', params.sortOrder)

  const url = `/api/v1/saved-filters?${queryParams.toString()}`

  const response = await apiClient.get<SavedFilterDto[]>(url)

  let pagination: PaginationInfo = {
    pageNumber: 1,
    totalPages: 1,
    pageSize: 100,
    totalCount: response.data.length,
    hasPrevious: false,
    hasNext: false,
  }

  const paginationHeader = response.headers['x-pagination']
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

export function useSavedFilters(context: string) {
  return useQuery({
    queryKey: SavedFilterKeys.list({
      context,
      pageSize: 100,
      sortOrder: 'Name',
    }),
    queryFn: () =>
      getSavedFilters({ context, pageSize: 100, sortOrder: 'Name' }),
  })
}
