import {
  startOfMonth,
  endOfMonth,
  startOfWeek,
  endOfWeek,
  addDays,
  eachDayOfInterval,
  format,
  isSameMonth,
  isToday,
} from 'date-fns'
import { useDroppable } from '@dnd-kit/core'
import { cn } from '@/lib/utils'
import type { MealPlanDayDto, MealPlanEntryDto } from '@/domain/meal-plans/types'
import type { MealPlanDensity } from '@/components/meal-plan/meal-plan-view-toggle'
import { MealPlanEntryCard } from '@/components/meal-plan/meal-plan-entry-card'
import { MealPlanDayColumn } from '@/components/meal-plan/meal-plan-day-column'
import { Add01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

interface MealPlanMonthViewProps {
  currentDate: Date
  days: MealPlanDayDto[]
  density: MealPlanDensity
  isMobile?: boolean
  onAddEntry: (date: string) => void
  onAddNote: (date: string) => void
  onEditEntry: (entry: MealPlanEntryDto) => void
  onCopyEntry: (entry: MealPlanEntryDto) => void
  onDeleteEntry: (id: string) => void
}

export function MealPlanMonthView({
  currentDate,
  days,
  density,
  isMobile = false,
  onAddEntry,
  onAddNote,
  onEditEntry,
  onCopyEntry,
  onDeleteEntry,
}: MealPlanMonthViewProps) {
  const daysByDate = new Map(days.map((d) => [d.date, d]))

  if (isMobile) {
    const monthDays = eachDayOfInterval({
      start: startOfMonth(currentDate),
      end: endOfMonth(currentDate),
    })

    return (
      <div className="space-y-2">
        {monthDays.map((date) => {
          const dateStr = format(date, 'yyyy-MM-dd')
          const dayData = daysByDate.get(dateStr)
          return (
            <MealPlanDayColumn
              key={dateStr}
              date={date}
              dateStr={dateStr}
              entries={dayData?.entries ?? []}
              density={density}
              onAddEntry={onAddEntry}
              onAddNote={onAddNote}
              onEditEntry={onEditEntry}
              onCopyEntry={onCopyEntry}
              onDeleteEntry={onDeleteEntry}
            />
          )
        })}
      </div>
    )
  }

  const monthStart = startOfMonth(currentDate)
  const monthEnd = endOfMonth(currentDate)
  const calendarStart = startOfWeek(monthStart, { weekStartsOn: 0 })
  const calendarEnd = endOfWeek(monthEnd, { weekStartsOn: 0 })

  const calendarDays: Date[] = []
  let d = calendarStart
  while (d <= calendarEnd) {
    calendarDays.push(d)
    d = addDays(d, 1)
  }

  const weeks: Date[][] = []
  for (let i = 0; i < calendarDays.length; i += 7) {
    weeks.push(calendarDays.slice(i, i + 7))
  }

  return (
    <div className="space-y-1">
      <div className="grid grid-cols-7 gap-1">
        {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map((day) => (
          <div
            key={day}
            className="text-center text-xs font-medium text-muted-foreground py-1"
          >
            {day}
          </div>
        ))}
      </div>
      {weeks.map((week, wi) => (
        <div key={wi} className="grid grid-cols-7 gap-1">
          {week.map((date) => {
            const dateStr = format(date, 'yyyy-MM-dd')
            const dayData = daysByDate.get(dateStr)
            const inMonth = isSameMonth(date, currentDate)
            return (
              <MonthDayCell
                key={dateStr}
                date={date}
                dateStr={dateStr}
                entries={dayData?.entries ?? []}
                inMonth={inMonth}
                density={density}
                onAddEntry={onAddEntry}
                onEditEntry={onEditEntry}
                onCopyEntry={onCopyEntry}
                onDeleteEntry={onDeleteEntry}
              />
            )
          })}
        </div>
      ))}
    </div>
  )
}

interface MonthDayCellProps {
  date: Date
  dateStr: string
  entries: MealPlanEntryDto[]
  inMonth: boolean
  density: MealPlanDensity
  onAddEntry: (date: string) => void
  onEditEntry: (entry: MealPlanEntryDto) => void
  onCopyEntry: (entry: MealPlanEntryDto) => void
  onDeleteEntry: (id: string) => void
}

function MonthDayCell({
  date,
  dateStr,
  entries,
  inMonth,
  density,
  onAddEntry,
  onEditEntry,
  onCopyEntry,
  onDeleteEntry,
}: MonthDayCellProps) {
  const { setNodeRef, isOver } = useDroppable({
    id: `day-${dateStr}`,
    data: { type: 'day', date: dateStr },
  })

  const today = isToday(date)
  const isNormal = density === 'normal'
  const maxVisible = isNormal ? 5 : 3
  const visibleEntries = entries.slice(0, maxVisible)
  const moreCount = entries.length - maxVisible

  return (
    <div
      ref={setNodeRef}
      className={cn(
        'rounded border p-1 transition-colors group',
        isNormal ? 'min-h-[200px]' : 'min-h-[140px]',
        inMonth ? 'bg-card' : 'bg-muted/30',
        isOver && 'ring-2 ring-primary/50 bg-primary/5',
        today && 'border-primary',
      )}
    >
      <div className="flex items-center justify-between mb-0.5">
        <span
          className={cn(
            'flex h-5 w-5 items-center justify-center rounded-full text-xs',
            today
              ? 'bg-primary text-primary-foreground font-semibold'
              : inMonth
                ? 'text-foreground'
                : 'text-muted-foreground',
          )}
        >
          {format(date, 'd')}
        </span>
        {inMonth && (
          <button
            type="button"
            onClick={() => onAddEntry(dateStr)}
            className="flex h-4 w-4 items-center justify-center rounded hover:bg-accent text-muted-foreground hover:text-foreground opacity-0 group-hover:opacity-100 hover:opacity-100 focus:opacity-100"
          >
            <HugeiconsIcon icon={Add01Icon} className="size-3" />
          </button>
        )}
      </div>
      <div className={cn('space-y-1', isNormal && 'space-y-1.5')}>
        {visibleEntries.map((entry) => (
          <MealPlanEntryCard
            key={entry.id}
            entry={entry}
            onEdit={onEditEntry}
            onCopy={onCopyEntry}
            onDelete={onDeleteEntry}
            compact={!isNormal}
            density={density}
          />
        ))}
        {moreCount > 0 && (
          <button
            type="button"
            className="w-full text-left text-xs text-muted-foreground hover:text-foreground px-1"
            onClick={() => onAddEntry(dateStr)}
          >
            +{moreCount} more
          </button>
        )}
      </div>
    </div>
  )
}
