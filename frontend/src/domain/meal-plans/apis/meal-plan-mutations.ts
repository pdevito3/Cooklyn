import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/api-client'
import type {
  MealPlanEntryDto,
  MealPlanEntryForCreationDto,
  MealPlanEntryForUpdateDto,
  MoveMealPlanEntryDto,
  CopyMealPlanEntryDto,
  MealPlanDayDto,
  MealPlanQueueDto,
  MealPlanQueueForCreationDto,
  MealPlanQueueForUpdateDto,
  MealPlanQueueItemForCreationDto,
  AddToCalendarFromQueueDto,
  BulkShoppingListFromMealPlanDto,
} from '@/domain/meal-plans/types'
import type { ShoppingListDto } from '@/domain/shopping-lists/types'
import { MealPlanKeys } from '@/domain/meal-plans/apis/meal-plan.keys'
import { ShoppingListKeys } from '@/domain/shopping-lists/apis/shopping-list.keys'

// Entry mutations

async function addMealPlanEntry(
  dto: MealPlanEntryForCreationDto,
): Promise<MealPlanEntryDto> {
  const response = await apiClient.post<MealPlanEntryDto>(
    '/api/v1/meal-plans/entries',
    dto,
  )
  return response.data
}

export function useAddMealPlanEntry() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: addMealPlanEntry,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.calendar() })
    },
  })
}

async function updateMealPlanEntry(params: {
  id: string
  dto: MealPlanEntryForUpdateDto
}): Promise<MealPlanEntryDto> {
  const response = await apiClient.put<MealPlanEntryDto>(
    `/api/v1/meal-plans/entries/${params.id}`,
    params.dto,
  )
  return response.data
}

export function useUpdateMealPlanEntry() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: updateMealPlanEntry,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.calendar() })
    },
  })
}

async function deleteMealPlanEntry(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/meal-plans/entries/${id}`)
}

export function useDeleteMealPlanEntry() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: deleteMealPlanEntry,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.calendar() })
    },
  })
}

async function moveMealPlanEntry(params: {
  id: string
  dto: MoveMealPlanEntryDto
}): Promise<MealPlanEntryDto> {
  const response = await apiClient.post<MealPlanEntryDto>(
    `/api/v1/meal-plans/entries/${params.id}/move`,
    params.dto,
  )
  return response.data
}

export function useMoveMealPlanEntry() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: moveMealPlanEntry,
    onMutate: async ({ id, dto }) => {
      await queryClient.cancelQueries({ queryKey: MealPlanKeys.calendar() })

      const previousData = queryClient.getQueriesData<MealPlanDayDto[]>({
        queryKey: MealPlanKeys.calendar(),
      })

      queryClient.setQueriesData<MealPlanDayDto[]>(
        { queryKey: MealPlanKeys.calendar() },
        (old) => {
          if (!old) return old
          let movedEntry: MealPlanEntryDto | undefined
          const updated = old.map((day) => ({
            ...day,
            entries: day.entries.filter((e) => {
              if (e.id === id) {
                movedEntry = e
                return false
              }
              return true
            }),
          }))
          if (!movedEntry) return old
          const updatedEntry = {
            ...movedEntry,
            date: dto.targetDate,
            sortOrder: dto.sortOrder,
          }
          const targetDay = updated.find((d) => d.date === dto.targetDate)
          if (targetDay) {
            targetDay.entries = [...targetDay.entries, updatedEntry].sort(
              (a, b) => a.sortOrder - b.sortOrder,
            )
          } else {
            updated.push({ date: dto.targetDate, entries: [updatedEntry] })
            updated.sort((a, b) => a.date.localeCompare(b.date))
          }
          return updated.filter((d) => d.entries.length > 0)
        },
      )

      return { previousData }
    },
    onError: (_err, _vars, context) => {
      if (context?.previousData) {
        for (const [queryKey, data] of context.previousData) {
          queryClient.setQueryData(queryKey, data)
        }
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.calendar() })
    },
  })
}

async function copyMealPlanEntry(params: {
  id: string
  dto: CopyMealPlanEntryDto
}): Promise<MealPlanEntryDto> {
  const response = await apiClient.post<MealPlanEntryDto>(
    `/api/v1/meal-plans/entries/${params.id}/copy`,
    params.dto,
  )
  return response.data
}

export function useCopyMealPlanEntry() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: copyMealPlanEntry,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.calendar() })
    },
  })
}

// Queue mutations

async function addMealPlanQueue(
  dto: MealPlanQueueForCreationDto,
): Promise<MealPlanQueueDto> {
  const response = await apiClient.post<MealPlanQueueDto>(
    '/api/v1/meal-plans/queues',
    dto,
  )
  return response.data
}

export function useAddMealPlanQueue() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: addMealPlanQueue,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.queues() })
    },
  })
}

async function updateMealPlanQueue(params: {
  id: string
  dto: MealPlanQueueForUpdateDto
}): Promise<MealPlanQueueDto> {
  const response = await apiClient.put<MealPlanQueueDto>(
    `/api/v1/meal-plans/queues/${params.id}`,
    params.dto,
  )
  return response.data
}

export function useUpdateMealPlanQueue() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: updateMealPlanQueue,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.queues() })
    },
  })
}

async function deleteMealPlanQueue(id: string): Promise<void> {
  await apiClient.delete(`/api/v1/meal-plans/queues/${id}`)
}

export function useDeleteMealPlanQueue() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: deleteMealPlanQueue,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.queues() })
    },
  })
}

async function addMealPlanQueueItem(params: {
  queueId: string
  dto: MealPlanQueueItemForCreationDto
}): Promise<MealPlanQueueDto> {
  const response = await apiClient.post<MealPlanQueueDto>(
    `/api/v1/meal-plans/queues/${params.queueId}/items`,
    params.dto,
  )
  return response.data
}

export function useAddMealPlanQueueItem() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: addMealPlanQueueItem,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.queues() })
    },
  })
}

async function deleteMealPlanQueueItem(params: {
  queueId: string
  itemId: string
}): Promise<MealPlanQueueDto> {
  const response = await apiClient.delete<MealPlanQueueDto>(
    `/api/v1/meal-plans/queues/${params.queueId}/items/${params.itemId}`,
  )
  return response.data
}

export function useDeleteMealPlanQueueItem() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: deleteMealPlanQueueItem,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.queues() })
    },
  })
}

// Add from queue to calendar

async function addToCalendarFromQueue(
  dto: AddToCalendarFromQueueDto,
): Promise<MealPlanEntryDto> {
  const response = await apiClient.post<MealPlanEntryDto>(
    '/api/v1/meal-plans/add-from-queue',
    dto,
  )
  return response.data
}

export function useAddToCalendarFromQueue() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: addToCalendarFromQueue,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MealPlanKeys.calendar() })
    },
  })
}

// Shopping list generation

async function generateShoppingList(
  dto: BulkShoppingListFromMealPlanDto,
): Promise<ShoppingListDto> {
  const response = await apiClient.post<ShoppingListDto>(
    '/api/v1/meal-plans/generate-shopping-list',
    dto,
  )
  return response.data
}

export function useGenerateShoppingList() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: generateShoppingList,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ShoppingListKeys.all,
      })
    },
  })
}
