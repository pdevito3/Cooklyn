import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type { MealPlanRecipeIngredientsDto } from '@/domain/meal-plans/types'
import { MealPlanKeys } from '@/domain/meal-plans/apis/meal-plan.keys'

async function getMealPlanCalendarIngredients(
  startDate: string,
  endDate: string,
): Promise<MealPlanRecipeIngredientsDto[]> {
  const response = await apiClient.get<MealPlanRecipeIngredientsDto[]>(
    '/api/v1/meal-plans/calendar-ingredients',
    { params: { startDate, endDate } },
  )
  return response.data
}

export function useMealPlanCalendarIngredients(
  startDate: string,
  endDate: string,
) {
  return useQuery({
    queryKey: MealPlanKeys.calendarIngredients(startDate, endDate),
    queryFn: () => getMealPlanCalendarIngredients(startDate, endDate),
    enabled: !!startDate && !!endDate,
  })
}
