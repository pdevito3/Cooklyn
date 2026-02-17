export interface StoreAisleDto {
  id: string
  storeId: string
  storeSectionId: string
  sortOrder: number
  customName: string | null
}

export interface StoreAisleForUpdateDto {
  storeSectionId: string
  sortOrder: number
  customName: string | null
}

export interface StoreDefaultCollectionDto {
  id: string
  itemCollectionId: string
  itemCollectionName: string
}

export interface StoreDto {
  id: string
  tenantId: string
  name: string
  address: string | null
  storeAisles: StoreAisleDto[]
  storeDefaultCollections: StoreDefaultCollectionDto[]
}

export interface StoreForCreationDto {
  name: string
  address: string | null
}

export interface StoreForUpdateDto {
  name: string
  address: string | null
}

export interface StoreParametersDto {
  pageNumber?: number
  pageSize?: number
  filters?: string
  sortOrder?: string
}
