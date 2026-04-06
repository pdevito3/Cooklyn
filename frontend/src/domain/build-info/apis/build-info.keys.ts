export const BuildInfoKeys = {
  all: ['build-info'] as const,
  detail: () => [...BuildInfoKeys.all, 'detail'] as const,
}
