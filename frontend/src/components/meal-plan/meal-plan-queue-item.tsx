import { useState } from 'react'
import { useDraggable } from '@dnd-kit/core'
import { CSS } from '@dnd-kit/utilities'
import type { MealPlanQueueItemDto } from '@/domain/meal-plans/types'
import { useAddMealPlanEntry } from '@/domain/meal-plans/apis/meal-plan-mutations'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogFooter,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Delete01Icon,
  StickyNote01Icon,
  MoreVerticalIcon,
  Calendar01Icon,
  Image01Icon,
  DragDropIcon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { format } from 'date-fns'

interface MealPlanQueueItemProps {
  item: MealPlanQueueItemDto
  onDelete: (itemId: string) => void
}

export function MealPlanQueueItem({ item, onDelete }: MealPlanQueueItemProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [addDialogOpen, setAddDialogOpen] = useState(false)
  const [targetDate, setTargetDate] = useState(() =>
    format(new Date(), 'yyyy-MM-dd'),
  )

  const addEntry = useAddMealPlanEntry()

  const { attributes, listeners, setNodeRef, transform, isDragging } =
    useDraggable({
      id: `queue-item-${item.id}`,
      data: { type: 'queue-item', queueItem: item },
    })

  const style = {
    transform: CSS.Translate.toString(transform),
    opacity: isDragging ? 0.5 : 1,
  }

  const isNote = !item.recipeId

  function handleAddToCalendar() {
    if (!targetDate) return
    addEntry.mutate(
      {
        date: targetDate,
        entryType: item.recipeId ? 'Recipe' : 'FreeText',
        recipeId: item.recipeId,
        title: item.title,
        scale: item.scale,
        sortOrder: 0,
      },
      {
        onSuccess: () => setAddDialogOpen(false),
      },
    )
  }

  return (
    <>
      {isNote ? (
        <div
          ref={setNodeRef}
          style={style}
          className="group flex items-center gap-2 rounded-md border bg-emerald-50 border-emerald-200 dark:bg-emerald-900/40 dark:border-emerald-700/50 dark:text-emerald-50 px-2 py-1.5 shadow-sm"
        >
          <button
            type="button"
            aria-label="Drag to move"
            className="inline-flex shrink-0 items-center justify-center text-muted-foreground hover:text-foreground cursor-grab active:cursor-grabbing touch-none -ml-0.5"
            {...listeners}
            {...attributes}
          >
            <HugeiconsIcon icon={DragDropIcon} className="size-4" />
          </button>
          <HugeiconsIcon
            icon={StickyNote01Icon}
            className="size-3.5 shrink-0 text-muted-foreground"
          />
          <span className="flex-1 truncate text-sm">{item.title}</span>
          <DropdownMenu>
            <DropdownMenuTrigger
              render={
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-6 w-6 shrink-0 md:opacity-0 md:group-hover:opacity-100"
                />
              }
            >
              <HugeiconsIcon icon={MoreVerticalIcon} className="size-3.5" />
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-44">
              <DropdownMenuItem onClick={() => setAddDialogOpen(true)}>
                <HugeiconsIcon icon={Calendar01Icon} className="size-4" />
                Add to Meal Plan
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                variant="destructive"
                onClick={() => setDeleteDialogOpen(true)}
              >
                <HugeiconsIcon icon={Delete01Icon} className="size-4" />
                Remove
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      ) : (
        <div
          ref={setNodeRef}
          style={style}
          className="group relative rounded-md border bg-card shadow-sm overflow-hidden"
        >
          <button
            type="button"
            aria-label="Drag to move"
            className="absolute top-1 left-1 z-10 inline-flex items-center justify-center rounded-md bg-background/80 backdrop-blur-sm p-1 text-muted-foreground hover:text-foreground cursor-grab active:cursor-grabbing touch-none shadow-sm"
            {...listeners}
            {...attributes}
          >
            <HugeiconsIcon icon={DragDropIcon} className="size-3.5" />
          </button>
          {item.imageUrl ? (
            <img
              src={item.imageUrl}
              alt=""
              className="h-24 w-full object-cover"
            />
          ) : (
            <div className="flex h-16 w-full items-center justify-center bg-muted">
              <HugeiconsIcon
                icon={Image01Icon}
                className="size-6 text-muted-foreground"
              />
            </div>
          )}
          <div className="flex items-start gap-1 p-2">
            <div className="flex-1 min-w-0">
              <span className="text-sm font-medium leading-tight line-clamp-2">
                {item.title}
              </span>
              {item.scale !== 1 && (
                <span className="text-xs text-muted-foreground">
                  {item.scale}x
                </span>
              )}
            </div>
            <DropdownMenu>
              <DropdownMenuTrigger
                render={
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-6 w-6 shrink-0 md:opacity-0 md:group-hover:opacity-100"
                  />
                }
              >
                <HugeiconsIcon icon={MoreVerticalIcon} className="size-3.5" />
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-44">
                <DropdownMenuItem onClick={() => setAddDialogOpen(true)}>
                  <HugeiconsIcon icon={Calendar01Icon} className="size-4" />
                  Add to Meal Plan
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem
                  variant="destructive"
                  onClick={() => setDeleteDialogOpen(true)}
                >
                  <HugeiconsIcon icon={Delete01Icon} className="size-4" />
                  Remove
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>
      )}

      {/* Add to meal plan date picker dialog */}
      <Dialog open={addDialogOpen} onOpenChange={setAddDialogOpen}>
        <DialogContent className="max-w-sm">
          <DialogHeader>
            <DialogTitle>Add to Meal Plan</DialogTitle>
          </DialogHeader>
          <div className="space-y-3 mt-2">
            <p className="text-sm text-muted-foreground">
              Add &ldquo;{item.title}&rdquo; to your meal plan.
            </p>
            <div className="space-y-2">
              <Label>Date</Label>
              <Input
                type="date"
                value={targetDate}
                onChange={(e) => setTargetDate(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter' && (e.metaKey || e.ctrlKey) && targetDate) {
                    e.preventDefault()
                    handleAddToCalendar()
                  }
                }}
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setAddDialogOpen(false)}>
              Cancel
            </Button>
            <Button
              onClick={handleAddToCalendar}
              disabled={!targetDate || addEntry.isPending}
            >
              {addEntry.isPending ? 'Adding...' : 'Add'}
              {!addEntry.isPending && (
                <kbd className="pointer-events-none ml-2 shrink-0 select-none inline-flex items-center justify-center rounded border bg-muted/50 px-1.5 py-0.5 font-sans text-xs font-semibold text-muted-foreground">
                  ⌘↵
                </kbd>
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete confirmation dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Remove Queue Item</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to remove &ldquo;{item.title}&rdquo; from
              the queue?
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={() => {
                onDelete(item.id)
                setDeleteDialogOpen(false)
              }}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              Remove
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  )
}
