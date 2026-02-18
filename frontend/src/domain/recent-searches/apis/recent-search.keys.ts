export const RecentSearchKeys = {
  all: ['recent-searches'] as const,
  lists: () => [...RecentSearchKeys.all, 'list'] as const,
  list: (params?: { pageSize?: number }) =>
    [...RecentSearchKeys.lists(), params] as const,
}
