import { useState, useEffect, useMemo, useCallback } from 'react'
import { format, startOfWeek, endOfWeek } from 'date-fns'
import { ArrowDown01Icon, ArrowRight01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
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
import { Kbd } from '@/components/ui/kbd'
import { Checkbox } from '@/components/ui/checkbox'
import {
  ShoppingListPicker,
  type ShoppingListPickerMode,
} from '@/components/shopping-list-picker'
import { useMealPlanCalendarIngredients } from '@/domain/meal-plans/apis/get-meal-plan-calendar-ingredients'
import { useGenerateShoppingList } from '@/domain/meal-plans/apis/meal-plan-mutations'
import type { MealPlanRecipeIngredientsDto } from '@/domain/meal-plans/types'
import { useNavigate } from '@tanstack/react-router'
import { Notification } from '@/components/notifications'

interface GenerateShoppingListDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function GenerateShoppingListDialog({
  open,
  onOpenChange,
}: GenerateShoppingListDialogProps) {
  const navigate = useNavigate()
  const today = new Date()
  const defaultStart = format(
    startOfWeek(today, { weekStartsOn: 0 }),
    'yyyy-MM-dd',
  )
  const defaultEnd = format(
    endOfWeek(today, { weekStartsOn: 0 }),
    'yyyy-MM-dd',
  )

  const [startDate, setStartDate] = useState(defaultStart)
  const [endDate, setEndDate] = useState(defaultEnd)

  // Shopping list picker state
  const [listMode, setListMode] = useState<ShoppingListPickerMode>('new')
  const [selectedListId, setSelectedListId] = useState<string | null>(null)
  const [newListName, setNewListName] = useState('')

  // Ingredient selection: entryId -> Set of selected ingredientIds
  const [selections, setSelections] = useState<Map<string, Set<string>>>(
    new Map(),
  )
  const [expandedEntries, setExpandedEntries] = useState<Set<string> | null>(
    null,
  )

  const { data: recipeEntries = [] } = useMealPlanCalendarIngredients(
    startDate,
    endDate,
  )
  const generateList = useGenerateShoppingList()

  // Initialize selections and expand all when data changes
  useEffect(() => {
    const newSelections = new Map<string, Set<string>>()
    const allEntryIds = new Set<string>()
    for (const entry of recipeEntries) {
      newSelections.set(
        entry.entryId,
        new Set(entry.ingredients.map((i) => i.id)),
      )
      allEntryIds.add(entry.entryId)
    }
    setSelections(newSelections)
    setExpandedEntries(allEntryIds)
  }, [recipeEntries])

  const toggleExpanded = (entryId: string) => {
    setExpandedEntries((prev) => {
      const next = new Set(prev)
      if (next.has(entryId)) {
        next.delete(entryId)
      } else {
        next.add(entryId)
      }
      return next
    })
  }

  const toggleEntry = useCallback(
    (entry: MealPlanRecipeIngredientsDto) => {
      setSelections((prev) => {
        const next = new Map(prev)
        const current = next.get(entry.entryId)
        const allIds = entry.ingredients.map((i) => i.id)
        if (current && current.size === allIds.length) {
          // Deselect all
          next.set(entry.entryId, new Set())
        } else {
          // Select all
          next.set(entry.entryId, new Set(allIds))
        }
        return next
      })
    },
    [],
  )

  const toggleIngredient = useCallback(
    (entryId: string, ingredientId: string) => {
      setSelections((prev) => {
        const next = new Map(prev)
        const current = new Set(next.get(entryId) ?? [])
        if (current.has(ingredientId)) {
          current.delete(ingredientId)
        } else {
          current.add(ingredientId)
        }
        next.set(entryId, current)
        return next
      })
    },
    [],
  )

  const totalSelectedIngredients = useMemo(() => {
    let count = 0
    for (const ids of selections.values()) {
      count += ids.size
    }
    return count
  }, [selections])

  const totalIngredients = useMemo(
    () => recipeEntries.reduce((sum, e) => sum + e.ingredients.length, 0),
    [recipeEntries],
  )

  const handleSubmit = () => {
    const entryIngredientSelections = Array.from(selections.entries())
      .filter(([, ids]) => ids.size > 0)
      .map(([entryId, ids]) => ({
        entryId,
        ingredientIds: Array.from(ids),
      }))

    generateList.mutate(
      {
        startDate,
        endDate,
        shoppingListId: listMode === 'existing' ? selectedListId : null,
        newShoppingListName: listMode === 'new' ? newListName || null : null,
        excludedEntryIds: null,
        entryIngredientSelections,
      },
      {
        onSuccess: (data) => {
          onOpenChange(false)
          Notification.success('Shopping list generated!')
          navigate({
            to: '/shopping-lists/$id',
            params: { id: data.id },
          })
        },
      },
    )
  }

  const canSubmit =
    totalSelectedIngredients > 0 &&
    !generateList.isPending &&
    (listMode === 'new' || selectedListId)

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg max-h-[85vh] flex flex-col">
        <DialogHeader>
          <DialogTitle>Generate Shopping List</DialogTitle>
        </DialogHeader>
        <div className="space-y-4 mt-2 overflow-y-auto flex-1 min-h-0">
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-2">
              <Label>Start Date</Label>
              <Input
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label>End Date</Label>
              <Input
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
              />
            </div>
          </div>

          <ShoppingListPicker
            mode={listMode}
            onModeChange={setListMode}
            selectedListId={selectedListId}
            onSelectedListIdChange={setSelectedListId}
            newListName={newListName}
            onNewListNameChange={setNewListName}
            newListPlaceholder={`Meal Plan ${startDate && endDate ? `${format(new Date(startDate), 'MMM d')} - ${format(new Date(endDate), 'MMM d')}` : ''}`}
          />

          {recipeEntries.length > 0 ? (
            <div className="space-y-1">
              <Label>
                Ingredients ({totalSelectedIngredients} of {totalIngredients}{' '}
                selected)
              </Label>
              <div className="max-h-64 overflow-y-auto rounded border">
                {recipeEntries.map((entry) => {
                  const selectedIds = selections.get(entry.entryId)
                  const selectedCount = selectedIds?.size ?? 0
                  const totalCount = entry.ingredients.length
                  const allSelected = selectedCount === totalCount
                  const someSelected =
                    selectedCount > 0 && selectedCount < totalCount
                  const isExpanded = expandedEntries?.has(entry.entryId) ?? false

                  return (
                    <div key={entry.entryId} className="border-b last:border-0">
                      <div className="flex items-center gap-2 px-3 py-2 hover:bg-muted/50">
                        <button
                          type="button"
                          onClick={() => toggleExpanded(entry.entryId)}
                          className="shrink-0 text-muted-foreground hover:text-foreground"
                        >
                          <HugeiconsIcon
                            icon={
                              isExpanded ? ArrowDown01Icon : ArrowRight01Icon
                            }
                            className="size-4"
                          />
                        </button>
                        <Checkbox
                          isSelected={allSelected}
                          isIndeterminate={someSelected}
                          onChange={() => toggleEntry(entry)}
                        >
                          <span className="text-sm font-medium">
                            {entry.recipeTitle}
                          </span>
                          {entry.scale !== 1 && (
                            <span className="text-muted-foreground text-xs ml-1">
                              ({entry.scale}x)
                            </span>
                          )}
                          <span className="text-xs text-muted-foreground ml-1">
                            - {entry.date}
                          </span>
                          <span className="text-xs text-muted-foreground ml-1">
                            ({selectedCount}/{totalCount})
                          </span>
                        </Checkbox>
                      </div>
                      {isExpanded && (
                        <div className="pl-11 pr-3 pb-2 space-y-1">
                          {entry.ingredients.map((ingredient) => (
                            <div
                              key={ingredient.id}
                              className="flex items-center gap-2"
                            >
                              <Checkbox
                                isSelected={
                                  selectedIds?.has(ingredient.id) ?? false
                                }
                                onChange={() =>
                                  toggleIngredient(
                                    entry.entryId,
                                    ingredient.id,
                                  )
                                }
                              >
                                <span className="text-sm">
                                  {ingredient.rawText ||
                                    ingredient.name ||
                                    'Unknown ingredient'}
                                </span>
                              </Checkbox>
                            </div>
                          ))}
                        </div>
                      )}
                    </div>
                  )
                })}
              </div>
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">
              No recipe entries found in the selected date range.
            </p>
          )}
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={!canSubmit}
          >
            {generateList.isPending ? 'Generating...' : 'Generate'}
            {!generateList.isPending && <Kbd>⌘↵</Kbd>}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
