import { useDroppable } from '@dnd-kit/core'
import { format, isToday } from 'date-fns'
import { cn } from '@/lib/utils'
import type { MealPlanEntryDto } from '@/domain/meal-plans/types'
import type { MealPlanDensity } from '@/components/meal-plan/meal-plan-view-toggle'
import { MealPlanEntryCard } from '@/components/meal-plan/meal-plan-entry-card'
import { Add01Icon, StickyNote01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

interface MealPlanDayColumnProps {
  date: Date
  dateStr: string
  entries: MealPlanEntryDto[]
  density: MealPlanDensity
  onAddEntry: (date: string) => void
  onAddNote: (date: string) => void
  onEditEntry: (entry: MealPlanEntryDto) => void
  onCopyEntry: (entry: MealPlanEntryDto) => void
  onDeleteEntry: (id: string) => void
}

export function MealPlanDayColumn({
  date,
  dateStr,
  entries,
  density,
  onAddEntry,
  onAddNote,
  onEditEntry,
  onCopyEntry,
  onDeleteEntry,
}: MealPlanDayColumnProps) {
  const { setNodeRef, isOver } = useDroppable({
    id: `day-${dateStr}`,
    data: { type: 'day', date: dateStr },
  })

  const today = isToday(date)

  return (
    <div
      ref={setNodeRef}
      className={cn(
        'flex flex-col rounded-lg border bg-card md:min-h-[400px]',
        isOver && 'ring-2 ring-primary/50 bg-primary/5',
        today && 'border-primary',
      )}
    >
      <div
        className={cn(
          'flex items-center justify-between px-3 py-2 border-b',
          today && 'bg-primary/10',
        )}
      >
        <div className="flex items-center gap-1.5">
          <span className="text-xs font-medium text-muted-foreground hidden md:inline">
            {format(date, 'EEE')}
          </span>
          <span className="text-sm font-semibold md:hidden">
            {format(date, 'EEE, MMM d')}
          </span>
          <span
            className={cn(
              'hidden md:flex h-6 w-6 items-center justify-center rounded-full text-xs font-semibold',
              today
                ? 'bg-primary text-primary-foreground'
                : 'text-foreground',
            )}
          >
            {format(date, 'd')}
          </span>
          {today && (
            <span className="md:hidden text-xs font-medium text-primary">
              Today
            </span>
          )}
        </div>
        <div className="flex items-center gap-1">
          <button
            type="button"
            onClick={() => onAddNote(dateStr)}
            className="flex h-7 w-7 items-center justify-center rounded-md hover:bg-accent text-muted-foreground hover:text-foreground"
            title="Add note"
          >
            <HugeiconsIcon icon={StickyNote01Icon} className="size-4" />
          </button>
          <button
            type="button"
            onClick={() => onAddEntry(dateStr)}
            className="flex h-7 w-7 items-center justify-center rounded-md hover:bg-accent text-muted-foreground hover:text-foreground"
            title="Add recipe"
          >
            <HugeiconsIcon icon={Add01Icon} className="size-4" />
          </button>
        </div>
      </div>
      <div className="flex-1 space-y-1 p-1.5">
        {entries.map((entry) => (
          <MealPlanEntryCard
            key={entry.id}
            entry={entry}
            density={density}
            onEdit={onEditEntry}
            onCopy={onCopyEntry}
            onDelete={onDeleteEntry}
          />
        ))}
      </div>
    </div>
  )
}
