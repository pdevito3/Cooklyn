export interface SavedFilterDto {
  id: string
  name: string
  context: string
  filterStateJson: string
}

export interface SavedFilterForCreationDto {
  name: string
  context: string
  filterStateJson: string
}

export interface SavedFilterForUpdateDto {
  name: string
  filterStateJson: string
}

export interface SavedFilterParametersDto {
  pageNumber?: number
  pageSize?: number
  filters?: string
  sortOrder?: string
  context: string
}
