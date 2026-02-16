export const UserKeys = {
  all: ['users'] as const,
  details: () => [...UserKeys.all, 'detail'] as const,
  detail: (id: string) => [...UserKeys.details(), id] as const,
  byIdentifier: (identifier: string) =>
    [...UserKeys.all, 'by-identifier', identifier] as const,
  myDefaultStore: () => [...UserKeys.all, 'my-default-store'] as const,
}
