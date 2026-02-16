import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { ItemCollectionDto, ItemCollectionParametersDto } from '../types'
import { ItemCollectionKeys } from './item-collection.keys'
import type { PaginationInfo } from '@/domain/recipes/apis/get-recipes'

export interface ItemCollectionListResponse {
  items: ItemCollectionDto[]
  pagination: PaginationInfo
}

export async function getItemCollections(
  params?: ItemCollectionParametersDto,
): Promise<ItemCollectionListResponse> {
  const queryParams = new URLSearchParams()

  if (params?.pageNumber)
    queryParams.set('pageNumber', params.pageNumber.toString())
  if (params?.pageSize) queryParams.set('pageSize', params.pageSize.toString())
  if (params?.filters) queryParams.set('filters', params.filters)
  if (params?.sortOrder) queryParams.set('sortOrder', params.sortOrder)

  const queryString = queryParams.toString()
  const url = `/api/v1/itemcollections${queryString ? `?${queryString}` : ''}`

  const response = await apiClient.get<ItemCollectionDto[]>(url)

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

export function useItemCollections(params?: ItemCollectionParametersDto) {
  return useQuery({
    queryKey: ItemCollectionKeys.list(params),
    queryFn: () => getItemCollections(params),
  })
}
