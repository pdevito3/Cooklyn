import { Button } from '@/components/ui/button'

export type MealPlanView = 'week' | 'month'
export type MealPlanDensity = 'condensed' | 'normal'

interface MealPlanViewToggleProps {
  view: MealPlanView
  onViewChange: (view: MealPlanView) => void
}

export function MealPlanViewToggle({
  view,
  onViewChange,
}: MealPlanViewToggleProps) {
  return (
    <div className="flex rounded-lg border bg-muted p-0.5">
      <Button
        variant={view === 'week' ? 'default' : 'ghost'}
        size="sm"
        className="h-7 px-3 text-xs"
        onClick={() => onViewChange('week')}
      >
        Week
      </Button>
      <Button
        variant={view === 'month' ? 'default' : 'ghost'}
        size="sm"
        className="h-7 px-3 text-xs"
        onClick={() => onViewChange('month')}
      >
        Month
      </Button>
    </div>
  )
}

interface MealPlanDensityToggleProps {
  density: MealPlanDensity
  onDensityChange: (density: MealPlanDensity) => void
}

export function MealPlanDensityToggle({
  density,
  onDensityChange,
}: MealPlanDensityToggleProps) {
  return (
    <div className="flex rounded-lg border bg-muted p-0.5">
      <Button
        variant={density === 'condensed' ? 'default' : 'ghost'}
        size="sm"
        className="h-7 px-3 text-xs"
        onClick={() => onDensityChange('condensed')}
      >
        Condensed
      </Button>
      <Button
        variant={density === 'normal' ? 'default' : 'ghost'}
        size="sm"
        className="h-7 px-3 text-xs"
        onClick={() => onDensityChange('normal')}
      >
        Normal
      </Button>
    </div>
  )
}
