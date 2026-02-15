export const StoreKeys = {
  all: ['stores'] as const,
  lists: () => [...StoreKeys.all, 'list'] as const,
  list: (params?: { pageNumber?: number; pageSize?: number; filters?: string; sortOrder?: string }) =>
    [...StoreKeys.lists(), params] as const,
  details: () => [...StoreKeys.all, 'detail'] as const,
  detail: (id: string) => [...StoreKeys.details(), id] as const,
}
