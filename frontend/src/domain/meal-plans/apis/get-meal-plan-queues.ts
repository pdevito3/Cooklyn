import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { MealPlanQueueDto } from '@/domain/meal-plans/types'
import { MealPlanKeys } from '@/domain/meal-plans/apis/meal-plan.keys'

export async function getMealPlanQueues(): Promise<MealPlanQueueDto[]> {
  const response = await apiClient.get<MealPlanQueueDto[]>(
    '/api/v1/meal-plans/queues',
  )
  return response.data
}

export function useMealPlanQueues() {
  return useQuery({
    queryKey: MealPlanKeys.queues(),
    queryFn: getMealPlanQueues,
  })
}
