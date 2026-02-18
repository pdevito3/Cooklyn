export interface RecentSearchDto {
  id: string
  searchType: 'query' | 'selection'
  searchText: string
  resourceType?: string | null
  resourceId?: string | null
  createdOn: string
}

export interface RecentSearchForCreationDto {
  searchType: 'query' | 'selection'
  searchText: string
  resourceType?: string | null
  resourceId?: string | null
}
