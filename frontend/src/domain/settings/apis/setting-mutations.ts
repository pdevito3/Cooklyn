import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import { SettingsKeys } from './settings.keys'

export async function upsertSetting(
  key: string,
  value: string | null,
): Promise<void> {
  await apiClient.put(`/api/v1/settings/${key}`, { value })
}

export function useUpdateDefaultStore() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (storeId: string | null) =>
      upsertSetting('default-store-id', storeId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: SettingsKeys.defaultStore() })
    },
  })
}
