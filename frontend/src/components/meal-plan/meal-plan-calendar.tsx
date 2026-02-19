import { useState, useCallback } from 'react'
import {
  startOfWeek,
  endOfWeek,
  addWeeks,
  subWeeks,
  addMonths,
  subMonths,
  startOfMonth,
  endOfMonth,
  format,
} from 'date-fns'
import { useMealPlanCalendar } from '@/domain/meal-plans/apis/get-meal-plan-calendar'
import {
  useDeleteMealPlanEntry,
  useCopyMealPlanEntry,
} from '@/domain/meal-plans/apis/meal-plan-mutations'
import type { MealPlanEntryDto } from '@/domain/meal-plans/types'
import { MealPlanDateNav } from '@/components/meal-plan/meal-plan-date-nav'
import {
  MealPlanViewToggle,
  MealPlanDensityToggle,
  type MealPlanView,
  type MealPlanDensity,
} from '@/components/meal-plan/meal-plan-view-toggle'
import { MealPlanWeekView } from '@/components/meal-plan/meal-plan-week-view'
import { MealPlanMonthView } from '@/components/meal-plan/meal-plan-month-view'
import { MealPlanEntryDialog } from '@/components/meal-plan/meal-plan-entry-dialog'
import { useIsMobile } from '@/hooks/use-mobile'

function getStoredView(): MealPlanView {
  try {
    const v = localStorage.getItem('meal-plan-view')
    return v === 'month' ? 'month' : 'week'
  } catch {
    return 'week'
  }
}

function getStoredDensity(): MealPlanDensity {
  try {
    const d = localStorage.getItem('meal-plan-density')
    return d === 'normal' ? 'normal' : 'condensed'
  } catch {
    return 'condensed'
  }
}

export function MealPlanCalendar() {
  const isMobile = useIsMobile()
  const [view, setView] = useState<MealPlanView>(getStoredView)
  const [density, setDensity] = useState<MealPlanDensity>(getStoredDensity)
  const [currentDate, setCurrentDate] = useState(() => new Date())
  const [dialogOpen, setDialogOpen] = useState(false)
  const [dialogDate, setDialogDate] = useState('')
  const [dialogMode, setDialogMode] = useState<'Recipe' | 'FreeText'>('Recipe')
  const [editEntry, setEditEntry] = useState<MealPlanEntryDto | null>(null)
  const [copyEntry, setCopyEntry] = useState<MealPlanEntryDto | null>(null)
  const [copyDialogOpen, setCopyDialogOpen] = useState(false)

  const weekStart = startOfWeek(currentDate, { weekStartsOn: 0 })
  const weekEnd = endOfWeek(currentDate, { weekStartsOn: 0 })

  const queryStart =
    view === 'week'
      ? format(weekStart, 'yyyy-MM-dd')
      : format(
          startOfWeek(startOfMonth(currentDate), { weekStartsOn: 0 }),
          'yyyy-MM-dd',
        )
  const queryEnd =
    view === 'week'
      ? format(weekEnd, 'yyyy-MM-dd')
      : format(
          endOfWeek(endOfMonth(currentDate), { weekStartsOn: 0 }),
          'yyyy-MM-dd',
        )

  const { data: days = [] } = useMealPlanCalendar(queryStart, queryEnd)
  const deleteEntry = useDeleteMealPlanEntry()
  const copyEntryMutation = useCopyMealPlanEntry()

  const handleViewChange = useCallback((v: MealPlanView) => {
    setView(v)
    localStorage.setItem('meal-plan-view', v)
  }, [])

  const handleDensityChange = useCallback((d: MealPlanDensity) => {
    setDensity(d)
    localStorage.setItem('meal-plan-density', d)
  }, [])

  const handlePrev = useCallback(() => {
    setCurrentDate((d) => (view === 'week' ? subWeeks(d, 1) : subMonths(d, 1)))
  }, [view])

  const handleNext = useCallback(() => {
    setCurrentDate((d) => (view === 'week' ? addWeeks(d, 1) : addMonths(d, 1)))
  }, [view])

  const handleToday = useCallback(() => {
    setCurrentDate(new Date())
  }, [])

  const handleAddEntry = useCallback((date: string) => {
    setDialogDate(date)
    setDialogMode('Recipe')
    setEditEntry(null)
    setDialogOpen(true)
  }, [])

  const handleAddNote = useCallback((date: string) => {
    setDialogDate(date)
    setDialogMode('FreeText')
    setEditEntry(null)
    setDialogOpen(true)
  }, [])

  const handleEditEntry = useCallback((entry: MealPlanEntryDto) => {
    setDialogDate(entry.date)
    setDialogMode(entry.entryType)
    setEditEntry(entry)
    setDialogOpen(true)
  }, [])

  const handleCopyEntry = useCallback((entry: MealPlanEntryDto) => {
    setCopyEntry(entry)
    setDialogDate(entry.date)
    setCopyDialogOpen(true)
  }, [])

  const handleDeleteEntry = useCallback(
    (id: string) => {
      deleteEntry.mutate(id)
    },
    [deleteEntry],
  )

  const handleConfirmCopy = useCallback(() => {
    if (!copyEntry || !dialogDate) return
    copyEntryMutation.mutate(
      {
        id: copyEntry.id,
        dto: { targetDate: dialogDate, sortOrder: 0 },
      },
      { onSuccess: () => setCopyDialogOpen(false) },
    )
  }, [copyEntry, dialogDate, copyEntryMutation])

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <MealPlanDateNav
          view={view}
          currentDate={currentDate}
          weekStart={weekStart}
          weekEnd={weekEnd}
          onPrev={handlePrev}
          onNext={handleNext}
          onToday={handleToday}
        />
        <div className="flex items-center gap-2">
          {!isMobile && (
            <MealPlanDensityToggle
              density={density}
              onDensityChange={handleDensityChange}
            />
          )}
          <MealPlanViewToggle view={view} onViewChange={handleViewChange} />
        </div>
      </div>

      {view === 'week' ? (
        <MealPlanWeekView
          weekStart={weekStart}
          days={days}
          density={isMobile ? 'normal' : density}
          isMobile={isMobile}
          onAddEntry={handleAddEntry}
          onAddNote={handleAddNote}
          onEditEntry={handleEditEntry}
          onCopyEntry={handleCopyEntry}
          onDeleteEntry={handleDeleteEntry}
        />
      ) : (
        <MealPlanMonthView
          currentDate={currentDate}
          days={days}
          density={isMobile ? 'normal' : density}
          isMobile={isMobile}
          onAddEntry={handleAddEntry}
          onAddNote={handleAddNote}
          onEditEntry={handleEditEntry}
          onCopyEntry={handleCopyEntry}
          onDeleteEntry={handleDeleteEntry}
        />
      )}

      <MealPlanEntryDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        date={dialogDate}
        editEntry={editEntry}
        initialMode={dialogMode}
      />

      {/* Copy date picker dialog */}
      {copyDialogOpen && copyEntry && (
        <CopyDateDialog
          open={copyDialogOpen}
          onOpenChange={setCopyDialogOpen}
          date={dialogDate}
          onDateChange={setDialogDate}
          onConfirm={handleConfirmCopy}
          isPending={copyEntryMutation.isPending}
          entryTitle={copyEntry.title}
        />
      )}
    </div>
  )
}

// Simple copy date dialog
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

function CopyDateDialog({
  open,
  onOpenChange,
  date,
  onDateChange,
  onConfirm,
  isPending,
  entryTitle,
}: {
  open: boolean
  onOpenChange: (v: boolean) => void
  date: string
  onDateChange: (d: string) => void
  onConfirm: () => void
  isPending: boolean
  entryTitle: string
}) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>Copy Entry</DialogTitle>
        </DialogHeader>
        <div className="space-y-3 mt-2">
          <p className="text-sm text-muted-foreground">
            Copy "{entryTitle}" to a new date.
          </p>
          <div className="space-y-2">
            <Label>Target Date</Label>
            <Input
              type="date"
              value={date}
              onChange={(e) => onDateChange(e.target.value)}
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button onClick={onConfirm} disabled={!date || isPending}>
            {isPending ? 'Copying...' : 'Copy'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
