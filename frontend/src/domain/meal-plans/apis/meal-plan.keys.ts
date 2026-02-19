export const MealPlanKeys = {
  all: ['meal-plans'] as const,
  calendar: () => [...MealPlanKeys.all, 'calendar'] as const,
  calendarRange: (startDate: string, endDate: string) =>
    [...MealPlanKeys.calendar(), { startDate, endDate }] as const,
  calendarIngredients: (startDate: string, endDate: string) =>
    [...MealPlanKeys.all, 'calendar-ingredients', { startDate, endDate }] as const,
  queues: () => [...MealPlanKeys.all, 'queues'] as const,
}
