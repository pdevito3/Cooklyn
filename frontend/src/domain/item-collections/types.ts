export interface ItemCollectionItemDto {
  id: string
  itemCollectionId: string
  name: string
  quantity: number | null
  unit: string | null
  storeSectionId: string | null
  sortOrder: number
}

export interface ItemCollectionDto {
  id: string
  tenantId: string
  name: string
  items: ItemCollectionItemDto[]
}

export interface ItemCollectionForCreationDto {
  name: string
}

export interface ItemCollectionForUpdateDto {
  name: string
}

export interface ItemCollectionItemForCreationDto {
  name: string
  quantity: number | null
  unit: string | null
  storeSectionId: string | null
  sortOrder: number
}

export interface ItemCollectionParametersDto {
  pageNumber?: number
  pageSize?: number
  filters?: string
  sortOrder?: string
}
