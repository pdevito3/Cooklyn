export const SavedFilterKeys = {
  all: ['saved-filters'] as const,
  lists: () => [...SavedFilterKeys.all, 'list'] as const,
  list: (params?: {
    pageNumber?: number
    pageSize?: number
    filters?: string
    sortOrder?: string
    context?: string
  }) => [...SavedFilterKeys.lists(), params] as const,
  details: () => [...SavedFilterKeys.all, 'detail'] as const,
  detail: (id: string) => [...SavedFilterKeys.details(), id] as const,
}
