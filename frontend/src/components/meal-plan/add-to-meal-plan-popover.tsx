import { useState } from 'react'
import { format } from 'date-fns'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogFooter,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { ScaleInput } from '@/components/scale-input'
import { Kbd } from '@/components/ui/kbd'
import {
  useAddMealPlanEntry,
  useAddMealPlanQueueItem,
} from '@/domain/meal-plans/apis/meal-plan-mutations'
import { useMealPlanQueues } from '@/domain/meal-plans/apis/get-meal-plan-queues'
import { Notification } from '@/components/notifications'

interface AddToMealPlanPopoverProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  recipeId: string
  recipeTitle: string
}

export function AddToMealPlanPopover({
  open,
  onOpenChange,
  recipeId,
  recipeTitle,
}: AddToMealPlanPopoverProps) {
  const [mode, setMode] = useState<'calendar' | 'queue'>('calendar')
  const [date, setDate] = useState(format(new Date(), 'yyyy-MM-dd'))
  const [scale, setScale] = useState(1)

  const addEntry = useAddMealPlanEntry()
  const addQueueItem = useAddMealPlanQueueItem()
  const { data: queues = [] } = useMealPlanQueues()

  const defaultQueue = queues.find((q) => q.isDefault)

  const handleAddToCalendar = () => {
    addEntry.mutate(
      {
        date,
        entryType: 'Recipe',
        recipeId,
        title: recipeTitle,
        scale,
        sortOrder: 0,
      },
      {
        onSuccess: () => {
          Notification.success(`Added "${recipeTitle}" to ${date}`)
          onOpenChange(false)
        },
      },
    )
  }

  const handleAddToQueue = () => {
    if (!defaultQueue) return
    addQueueItem.mutate(
      {
        queueId: defaultQueue.id,
        dto: { recipeId, title: recipeTitle, scale },
      },
      {
        onSuccess: () => {
          Notification.success(`Added "${recipeTitle}" to queue`)
          onOpenChange(false)
        },
      },
    )
  }

  const isPending = addEntry.isPending || addQueueItem.isPending

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>Add to Meal Plan</DialogTitle>
        </DialogHeader>
        <div className="space-y-4 mt-2">
          <p className="text-sm text-muted-foreground truncate">
            {recipeTitle}
          </p>

          <div className="flex rounded-lg border bg-muted p-0.5">
            <Button
              variant={mode === 'calendar' ? 'default' : 'ghost'}
              size="sm"
              className="flex-1 h-7 text-xs"
              onClick={() => setMode('calendar')}
            >
              Calendar
            </Button>
            <Button
              variant={mode === 'queue' ? 'default' : 'ghost'}
              size="sm"
              className="flex-1 h-7 text-xs"
              onClick={() => setMode('queue')}
            >
              Queue
            </Button>
          </div>

          {mode === 'calendar' && (
            <div className="space-y-2">
              <Label>Date</Label>
              <Input
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
              />
            </div>
          )}

          <div className="space-y-2">
            <Label>Scale</Label>
            <ScaleInput value={scale} onChange={setScale} />
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button
            onClick={
              mode === 'calendar' ? handleAddToCalendar : handleAddToQueue
            }
            disabled={isPending || (mode === 'queue' && !defaultQueue)}
          >
            {isPending
              ? 'Adding...'
              : mode === 'calendar'
                ? 'Add to Calendar'
                : 'Add to Queue'}
            {!isPending && <Kbd>⌘↵</Kbd>}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
