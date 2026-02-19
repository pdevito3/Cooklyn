import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { MealPlanDayDto } from '@/domain/meal-plans/types'
import { MealPlanKeys } from '@/domain/meal-plans/apis/meal-plan.keys'

export async function getMealPlanCalendar(
  startDate: string,
  endDate: string,
): Promise<MealPlanDayDto[]> {
  const response = await apiClient.get<MealPlanDayDto[]>(
    `/api/v1/meal-plans/calendar?startDate=${startDate}&endDate=${endDate}`,
  )
  return response.data
}

export function useMealPlanCalendar(startDate: string, endDate: string) {
  return useQuery({
    queryKey: MealPlanKeys.calendarRange(startDate, endDate),
    queryFn: () => getMealPlanCalendar(startDate, endDate),
    enabled: !!startDate && !!endDate,
  })
}
