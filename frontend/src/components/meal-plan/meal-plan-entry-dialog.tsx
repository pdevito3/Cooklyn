import { useState, useEffect, useRef, useCallback } from 'react'
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
import { useAddMealPlanEntry, useUpdateMealPlanEntry } from '@/domain/meal-plans/apis/meal-plan-mutations'
import type { MealPlanEntryDto } from '@/domain/meal-plans/types'
import { apiClient } from '@/lib/api-client'
import { useDebouncedValue } from '@/hooks/use-debounced-value'
import type { RecipeSummaryDto } from '@/domain/recipes/types'

interface MealPlanEntryDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  date: string
  editEntry?: MealPlanEntryDto | null
  sortOrder?: number
  initialMode?: 'Recipe' | 'FreeText'
}

export function MealPlanEntryDialog({
  open,
  onOpenChange,
  date,
  editEntry,
  sortOrder = 0,
  initialMode = 'Recipe',
}: MealPlanEntryDialogProps) {
  const [entryType, setEntryType] = useState<'Recipe' | 'FreeText'>(initialMode)
  const [title, setTitle] = useState('')
  const [recipeId, setRecipeId] = useState<string | null>(null)
  const [scale, setScale] = useState(1)
  const [recipeQuery, setRecipeQuery] = useState('')
  const [recipeResults, setRecipeResults] = useState<RecipeSummaryDto[]>([])
  const [showResults, setShowResults] = useState(false)
  const debouncedQuery = useDebouncedValue(recipeQuery, 250)
  const inputRef = useRef<HTMLInputElement>(null)
  const noteRef = useRef<HTMLTextAreaElement>(null)
  const resultsRef = useRef<HTMLDivElement>(null)

  const addEntry = useAddMealPlanEntry()
  const updateEntry = useUpdateMealPlanEntry()

  const isEditing = !!editEntry

  useEffect(() => {
    if (open && editEntry) {
      setEntryType(editEntry.entryType)
      setTitle(editEntry.title)
      setRecipeId(editEntry.recipeId)
      setScale(editEntry.scale)
      setRecipeQuery(editEntry.entryType === 'Recipe' ? editEntry.title : '')
    } else if (open) {
      setEntryType(initialMode)
      setTitle('')
      setRecipeId(null)
      setScale(1)
      setRecipeQuery('')
      setRecipeResults([])
    }
  }, [open, editEntry, initialMode])

  useEffect(() => {
    if (open) {
      const timer = setTimeout(() => {
        if (entryType === 'FreeText') {
          noteRef.current?.focus()
        } else {
          inputRef.current?.focus()
        }
      }, 100)
      return () => clearTimeout(timer)
    }
  }, [open, entryType])

  // Search recipes
  useEffect(() => {
    if (debouncedQuery.length < 2 || entryType !== 'Recipe') {
      setRecipeResults([])
      return
    }
    let cancelled = false
    apiClient
      .get(`/api/v1/recipes?filters=title @=* "${debouncedQuery}"&pageSize=8`)
      .then((res) => {
        if (!cancelled) {
          setRecipeResults(res.data as RecipeSummaryDto[])
          setShowResults(true)
        }
      })
      .catch(() => {})
    return () => {
      cancelled = true
    }
  }, [debouncedQuery, entryType])

  // Click outside to close results
  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (
        resultsRef.current &&
        !resultsRef.current.contains(e.target as Node) &&
        inputRef.current &&
        !inputRef.current.contains(e.target as Node)
      ) {
        setShowResults(false)
      }
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  const selectRecipe = useCallback((recipe: RecipeSummaryDto) => {
    setRecipeId(recipe.id)
    setTitle(recipe.title)
    setRecipeQuery(recipe.title)
    setShowResults(false)
  }, [])

  const handleSubmit = () => {
    const finalTitle = entryType === 'Recipe' ? title || recipeQuery : title
    if (!finalTitle.trim()) return

    if (isEditing && editEntry) {
      updateEntry.mutate(
        {
          id: editEntry.id,
          dto: { title: finalTitle, scale, sortOrder: editEntry.sortOrder },
        },
        { onSuccess: () => onOpenChange(false) },
      )
    } else {
      addEntry.mutate(
        {
          date,
          entryType,
          recipeId: entryType === 'Recipe' ? recipeId : null,
          title: finalTitle,
          scale: entryType === 'FreeText' ? 1 : scale,
          sortOrder,
        },
        { onSuccess: () => onOpenChange(false) },
      )
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && (e.metaKey || e.ctrlKey) && canSubmit) {
      e.preventDefault()
      handleSubmit()
    }
  }

  const isPending = addEntry.isPending || updateEntry.isPending
  const canSubmit =
    (entryType === 'FreeText' ? title.trim() : (title || recipeQuery).trim()) &&
    !isPending

  const isNote = entryType === 'FreeText'

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>
            {isEditing
              ? isNote
                ? 'Edit Note'
                : 'Edit Entry'
              : isNote
                ? 'Add Note'
                : 'Add to Meal Plan'}
          </DialogTitle>
        </DialogHeader>
        <div className="space-y-4 mt-2">
          {!isEditing && (
            <div className="flex rounded-lg border bg-muted p-0.5">
              <Button
                variant={entryType === 'Recipe' ? 'default' : 'ghost'}
                size="sm"
                className="flex-1 h-7 text-xs"
                onClick={() => setEntryType('Recipe')}
              >
                Recipe
              </Button>
              <Button
                variant={entryType === 'FreeText' ? 'default' : 'ghost'}
                size="sm"
                className="flex-1 h-7 text-xs"
                onClick={() => setEntryType('FreeText')}
              >
                Note
              </Button>
            </div>
          )}

          {entryType === 'Recipe' ? (
            <div className="space-y-2">
              <Label>Recipe</Label>
              <div className="relative">
                <Input
                  ref={inputRef}
                  placeholder="Search recipes..."
                  value={recipeQuery}
                  onChange={(e) => {
                    setRecipeQuery(e.target.value)
                    setRecipeId(null)
                    setTitle('')
                    setShowResults(true)
                  }}
                  onFocus={() => {
                    if (recipeResults.length > 0) setShowResults(true)
                  }}
                  onKeyDown={handleKeyDown}
                />
                {showResults && recipeResults.length > 0 && (
                  <div
                    ref={resultsRef}
                    className="absolute top-full left-0 right-0 z-50 mt-1 max-h-48 overflow-y-auto rounded-md border bg-popover shadow-md"
                  >
                    {recipeResults.map((recipe) => (
                      <button
                        key={recipe.id}
                        type="button"
                        className="flex w-full items-center gap-2 px-3 py-2 text-sm hover:bg-accent text-left"
                        onClick={() => selectRecipe(recipe)}
                      >
                        {recipe.imageUrl ? (
                          <img
                            src={recipe.imageUrl}
                            alt=""
                            className="size-6 shrink-0 rounded object-cover"
                          />
                        ) : (
                          <div className="size-6 shrink-0 rounded bg-muted" />
                        )}
                        <span className="truncate">{recipe.title}</span>
                      </button>
                    ))}
                  </div>
                )}
              </div>
              {recipeId && (
                <p className="text-xs text-muted-foreground">
                  Selected: {title}
                </p>
              )}
            </div>
          ) : (
            <div className="space-y-2">
              <Label>Note</Label>
              <textarea
                ref={noteRef}
                placeholder="e.g. Leftovers, Eating out, Prep ingredients..."
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                onKeyDown={handleKeyDown}
                className="flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 resize-none"
                rows={3}
              />
            </div>
          )}

          {entryType === 'Recipe' && (
            <div className="space-y-2">
              <Label>Scale</Label>
              <ScaleInput value={scale} onChange={setScale} />
            </div>
          )}
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={!canSubmit}>
            {isPending ? 'Saving...' : isEditing ? 'Update' : 'Add'}
            {!isPending && (
              <kbd className="pointer-events-none ml-2 shrink-0 select-none inline-flex items-center justify-center rounded border bg-muted/50 px-1.5 py-0.5 font-sans text-xs font-semibold text-muted-foreground">
                ⌘↵
              </kbd>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
