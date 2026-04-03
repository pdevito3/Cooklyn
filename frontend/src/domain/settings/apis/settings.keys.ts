export const SettingsKeys = {
  all: ['settings'] as const,
  detail: (key: string) => [...SettingsKeys.all, key] as const,
  defaultStore: () => SettingsKeys.detail('default-store-id'),
}
