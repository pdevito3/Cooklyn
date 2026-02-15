export const StoreSectionKeys = {
  all: ['store-sections'] as const,
  lists: () => [...StoreSectionKeys.all, 'list'] as const,
  list: (params?: { pageNumber?: number; pageSize?: number; filters?: string; sortOrder?: string }) =>
    [...StoreSectionKeys.lists(), params] as const,
  details: () => [...StoreSectionKeys.all, 'detail'] as const,
  detail: (id: string) => [...StoreSectionKeys.details(), id] as const,
}
