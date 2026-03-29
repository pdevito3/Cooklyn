import { createFileRoute } from '@tanstack/react-router'
import { useState } from 'react'
import {
  DndContext,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
  DragOverlay,
} from '@dnd-kit/core'
import { MealPlanCalendar } from '@/components/meal-plan/meal-plan-calendar'
import { MealPlanQueuePanel } from '@/components/meal-plan/meal-plan-queue-panel'
import { GenerateShoppingListDialog } from '@/components/meal-plan/generate-shopping-list-dialog'
import {
  useMoveMealPlanEntry,
  useAddMealPlanEntry,
} from '@/domain/meal-plans/apis/meal-plan-mutations'
import type {
  MealPlanEntryDto,
  MealPlanQueueItemDto,
} from '@/domain/meal-plans/types'
import { Button } from '@/components/ui/button'
import {
  ShoppingCart01Icon,
  StickyNote01Icon,
  RestaurantIcon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { Kbd } from '@/components/ui/kbd'
import { useHotkeys } from 'react-hotkeys-hook'
import { useIsMobile } from '@/hooks/use-mobile'

export const Route = createFileRoute('/meal-plan/')({
  component: MealPlanPage,
})

function MealPlanPage() {
  const isMobile = useIsMobile()
  const [queueOpen, setQueueOpen] = useState(!isMobile)
  const [shoppingListDialogOpen, setShoppingListDialogOpen] = useState(false)
  const [activeDragItem, setActiveDragItem] = useState<{
    type: 'entry' | 'queue-item'
    entry?: MealPlanEntryDto
    queueItem?: MealPlanQueueItemDto
  } | null>(null)

  const moveEntry = useMoveMealPlanEntry()
  const addEntry = useAddMealPlanEntry()

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 5 } }),
  )

  useHotkeys('q', () => setQueueOpen((v) => !v))
  useHotkeys('s', () => setShoppingListDialogOpen(true))

  function handleDragStart(event: {
    active: {
      data: {
        current?: {
          type?: string
          entry?: MealPlanEntryDto
          queueItem?: MealPlanQueueItemDto
        }
      }
    }
  }) {
    const data = event.active.data.current
    if (data?.type === 'entry' && data.entry) {
      setActiveDragItem({ type: 'entry', entry: data.entry })
    } else if (data?.type === 'queue-item' && data.queueItem) {
      setActiveDragItem({ type: 'queue-item', queueItem: data.queueItem })
    }
  }

  function handleDragEnd(event: DragEndEvent) {
    setActiveDragItem(null)
    const { active, over } = event
    if (!over) return

    const overData = over.data.current as
      | { type?: string; date?: string }
      | undefined
    if (overData?.type !== 'day' || !overData.date) return

    const activeData = active.data.current as
      | {
          type?: string
          entry?: MealPlanEntryDto
          queueItem?: MealPlanQueueItemDto
        }
      | undefined

    if (activeData?.type === 'entry' && activeData.entry) {
      if (activeData.entry.date === overData.date) return
      moveEntry.mutate({
        id: activeData.entry.id,
        dto: { targetDate: overData.date, sortOrder: 0 },
      })
    } else if (activeData?.type === 'queue-item' && activeData.queueItem) {
      const qi = activeData.queueItem
      addEntry.mutate({
        date: overData.date,
        entryType: qi.recipeId ? 'Recipe' : 'FreeText',
        recipeId: qi.recipeId,
        title: qi.title,
        scale: qi.scale,
        sortOrder: 0,
      })
    }
  }

  const dragLabel = activeDragItem?.entry
    ? activeDragItem.entry.title
    : activeDragItem?.queueItem?.title
  const dragIsNote = activeDragItem?.entry
    ? activeDragItem.entry.entryType === 'FreeText'
    : activeDragItem?.queueItem
      ? !activeDragItem.queueItem.recipeId
      : false
  const dragScale = activeDragItem?.entry
    ? activeDragItem.entry.scale
    : activeDragItem?.queueItem?.scale ?? 1

  return (
    <DndContext
      sensors={sensors}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
    >
      <div className="flex flex-col gap-4">
        <div className="min-w-0">
          <div className="flex flex-wrap items-center justify-end gap-2 mb-4">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setShoppingListDialogOpen(true)}
            >
              <HugeiconsIcon icon={ShoppingCart01Icon} className="size-4" />
              {!isMobile && 'Generate Shopping List'}
              {!isMobile && <Kbd>S</Kbd>}
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setQueueOpen((v) => !v)}
            >
              {queueOpen ? 'Hide Queue' : 'Show Queue'}
              {!isMobile && <Kbd>Q</Kbd>}
            </Button>
          </div>
          <MealPlanCalendar />
        </div>
        {queueOpen && <MealPlanQueuePanel />}
        <GenerateShoppingListDialog
          open={shoppingListDialogOpen}
          onOpenChange={setShoppingListDialogOpen}
        />
      </div>
      <DragOverlay>
        {activeDragItem && dragLabel && (
          <div className="flex items-center gap-1.5 rounded-md border bg-card px-2 py-1.5 text-sm shadow-lg">
            <HugeiconsIcon
              icon={dragIsNote ? StickyNote01Icon : RestaurantIcon}
              className="size-3.5 shrink-0 text-muted-foreground"
            />
            {dragLabel}
            {dragScale !== 1 && (
              <span className="ml-1 text-xs text-muted-foreground">
                {dragScale}x
              </span>
            )}
          </div>
        )}
      </DragOverlay>
    </DndContext>
  )
}
