import { format } from 'date-fns'
import { Button } from '@/components/ui/button'
import {
  ArrowLeft01Icon,
  ArrowRight01Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import type { MealPlanView } from '@/components/meal-plan/meal-plan-view-toggle'

interface MealPlanDateNavProps {
  view: MealPlanView
  currentDate: Date
  weekStart: Date
  weekEnd: Date
  onPrev: () => void
  onNext: () => void
  onToday: () => void
}

export function MealPlanDateNav({
  view,
  currentDate,
  weekStart,
  weekEnd,
  onPrev,
  onNext,
  onToday,
}: MealPlanDateNavProps) {
  const label =
    view === 'week'
      ? weekStart.getMonth() === weekEnd.getMonth()
        ? `${format(weekStart, 'MMM d')} - ${format(weekEnd, 'd, yyyy')}`
        : `${format(weekStart, 'MMM d')} - ${format(weekEnd, 'MMM d, yyyy')}`
      : format(currentDate, 'MMMM yyyy')

  return (
    <div className="flex items-center gap-2">
      <Button variant="outline" size="sm" onClick={onToday}>
        Today
      </Button>
      <Button variant="ghost" size="icon" className="h-8 w-8" onClick={onPrev}>
        <HugeiconsIcon icon={ArrowLeft01Icon} className="size-4" />
      </Button>
      <Button variant="ghost" size="icon" className="h-8 w-8" onClick={onNext}>
        <HugeiconsIcon icon={ArrowRight01Icon} className="size-4" />
      </Button>
      <h2 className="text-lg font-semibold">{label}</h2>
    </div>
  )
}
