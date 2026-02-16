import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { StoreDto, StoreParametersDto } from '../types'
import { StoreKeys } from './store.keys'
import type { PaginationInfo } from '@/domain/recipes/apis/get-recipes'

export interface StoreListResponse {
  items: StoreDto[]
  pagination: PaginationInfo
}

export async function getStores(
  params?: StoreParametersDto,
): Promise<StoreListResponse> {
  const queryParams = new URLSearchParams()

  if (params?.pageNumber)
    queryParams.set('pageNumber', params.pageNumber.toString())
  if (params?.pageSize) queryParams.set('pageSize', params.pageSize.toString())
  if (params?.filters) queryParams.set('filters', params.filters)
  if (params?.sortOrder) queryParams.set('sortOrder', params.sortOrder)

  const queryString = queryParams.toString()
  const url = `/api/v1/stores${queryString ? `?${queryString}` : ''}`

  const response = await apiClient.get<StoreDto[]>(url)

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

export function useStores(params?: StoreParametersDto) {
  return useQuery({
    queryKey: StoreKeys.list(params),
    queryFn: () => getStores(params),
  })
}
