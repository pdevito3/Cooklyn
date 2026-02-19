import { addDays, format } from 'date-fns'
import type { MealPlanDayDto, MealPlanEntryDto } from '@/domain/meal-plans/types'
import type { MealPlanDensity } from '@/components/meal-plan/meal-plan-view-toggle'
import { MealPlanDayColumn } from '@/components/meal-plan/meal-plan-day-column'

interface MealPlanWeekViewProps {
  weekStart: Date
  days: MealPlanDayDto[]
  density: MealPlanDensity
  isMobile?: boolean
  onAddEntry: (date: string) => void
  onAddNote: (date: string) => void
  onEditEntry: (entry: MealPlanEntryDto) => void
  onCopyEntry: (entry: MealPlanEntryDto) => void
  onDeleteEntry: (id: string) => void
}

export function MealPlanWeekView({
  weekStart,
  days,
  density,
  isMobile = false,
  onAddEntry,
  onAddNote,
  onEditEntry,
  onCopyEntry,
  onDeleteEntry,
}: MealPlanWeekViewProps) {
  const daysByDate = new Map(days.map((d) => [d.date, d]))
  const weekDays = Array.from({ length: 7 }, (_, i) => addDays(weekStart, i))

  if (isMobile) {
    return (
      <div className="space-y-2">
        {weekDays.map((date) => {
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

  return (
    <div className="grid grid-cols-7 gap-2">
      {weekDays.map((date) => {
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
