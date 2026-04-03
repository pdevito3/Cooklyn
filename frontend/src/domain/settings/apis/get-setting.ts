import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import { SettingsKeys } from './settings.keys'

export async function getSetting(key: string): Promise<string | null> {
  const response = await apiClient.get<{ value: string | null }>(
    `/api/v1/settings/${key}`,
  )
  return response.data.value
}

export function useDefaultStore() {
  return useQuery({
    queryKey: SettingsKeys.defaultStore(),
    queryFn: () => getSetting('default-store-id'),
    retry: false,
  })
}
