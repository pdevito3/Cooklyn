export const TagKeys = {
  all: ['tags'] as const,
  lists: () => [...TagKeys.all, 'list'] as const,
  list: (params?: {
    pageNumber?: number
    pageSize?: number
    filters?: string
    sortOrder?: string
  }) => [...TagKeys.lists(), params] as const,
  details: () => [...TagKeys.all, 'detail'] as const,
  detail: (id: string) => [...TagKeys.details(), id] as const,
}
