export interface TagDto {
  id: string
  tenantId: string
  name: string
}

export interface TagParametersDto {
  pageNumber?: number
  pageSize?: number
  filters?: string
  sortOrder?: string
}
