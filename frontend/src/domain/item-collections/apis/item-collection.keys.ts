export const ItemCollectionKeys = {
  all: ['item-collections'] as const,
  lists: () => [...ItemCollectionKeys.all, 'list'] as const,
  list: (params?: {
    pageNumber?: number
    pageSize?: number
    filters?: string
    sortOrder?: string
  }) => [...ItemCollectionKeys.lists(), params] as const,
  details: () => [...ItemCollectionKeys.all, 'detail'] as const,
  detail: (id: string) => [...ItemCollectionKeys.details(), id] as const,
}
