export interface StoreSectionDto {
  id: string
  tenantId: string
  name: string
}

export interface StoreSectionForCreationDto {
  name: string
}

export interface StoreSectionForUpdateDto {
  name: string
}

export interface StoreSectionParametersDto {
  pageNumber?: number
  pageSize?: number
  filters?: string
  sortOrder?: string
}
