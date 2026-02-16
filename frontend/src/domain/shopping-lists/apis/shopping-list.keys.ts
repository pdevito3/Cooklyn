export const ShoppingListKeys = {
  all: ['shopping-lists'] as const,
  lists: () => [...ShoppingListKeys.all, 'list'] as const,
  list: (params?: {
    pageNumber?: number
    pageSize?: number
    filters?: string
    sortOrder?: string
  }) => [...ShoppingListKeys.lists(), params] as const,
  details: () => [...ShoppingListKeys.all, 'detail'] as const,
  detail: (id: string) => [...ShoppingListKeys.details(), id] as const,
}
