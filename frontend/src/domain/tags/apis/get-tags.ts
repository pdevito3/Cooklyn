import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { TagDto, TagParametersDto } from '../types'
import { TagKeys } from './tag.keys'

export interface PaginationInfo {
  pageNumber: number
  totalPages: number
  pageSize: number
  totalCount: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface TagListResponse {
  items: TagDto[]
  pagination: PaginationInfo
}

export async function getTags(
  params?: TagParametersDto,
): Promise<TagListResponse> {
  const queryParams = new URLSearchParams()

  if (params?.pageNumber)
    queryParams.set('pageNumber', params.pageNumber.toString())
  if (params?.pageSize) queryParams.set('pageSize', params.pageSize.toString())
  if (params?.filters) queryParams.set('filters', params.filters)
  if (params?.sortOrder) queryParams.set('sortOrder', params.sortOrder)

  const queryString = queryParams.toString()
  const url = `/api/v1/tags${queryString ? `?${queryString}` : ''}`

  const response = await apiClient.get<TagDto[]>(url)

  let pagination: PaginationInfo = {
    pageNumber: 1,
    totalPages: 1,
    pageSize: 10,
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

export function useTags(params?: TagParametersDto) {
  return useQuery({
    queryKey: TagKeys.list(params),
    queryFn: () => getTags(params),
  })
}
