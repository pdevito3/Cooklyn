import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { BuildInfo } from '../types'
import { BuildInfoKeys } from './build-info.keys'

export async function getBuildInfo(): Promise<BuildInfo> {
  const response = await apiClient.get<BuildInfo>('/api/v1/buildinfo')
  return response.data
}

export function useBuildInfo() {
  return useQuery({
    queryKey: BuildInfoKeys.detail(),
    queryFn: getBuildInfo,
    staleTime: Infinity,
  })
}
