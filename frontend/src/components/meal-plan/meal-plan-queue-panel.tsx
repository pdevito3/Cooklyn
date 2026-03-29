import { useState, useRef, useEffect } from 'react'
import { useMealPlanQueues } from '@/domain/meal-plans/apis/get-meal-plan-queues'
import {
  useAddMealPlanQueueItem,
  useDeleteMealPlanQueueItem,
  useAddMealPlanQueue,
  useDeleteMealPlanQueue,
} from '@/domain/meal-plans/apis/meal-plan-mutations'
import { MealPlanQueueItem } from '@/components/meal-plan/meal-plan-queue-item'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
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
  Add01Icon,
  Delete01Icon,
  ArrowRight01Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import { apiClient } from '@/lib/api-client'
import { useDebouncedValue } from '@/hooks/use-debounced-value'
import type { RecipeSummaryDto } from '@/domain/recipes/types'

type AddItemMode = 'Recipe' | 'FreeText'

export function MealPlanQueuePanel() {
  const { data: queues = [] } = useMealPlanQueues()
  const addQueueItem = useAddMealPlanQueueItem()
  const deleteQueueItem = useDeleteMealPlanQueueItem()
  const addQueue = useAddMealPlanQueue()
  const deleteQueue = useDeleteMealPlanQueue()
  const [addingToQueue, setAddingToQueue] = useState<string | null>(null)
  const [addMode, setAddMode] = useState<AddItemMode>('Recipe')
  const [noteTitle, setNoteTitle] = useState('')
  const [recipeQuery, setRecipeQuery] = useState('')
  const [selectedRecipe, setSelectedRecipe] = useState<{
    id: string
    title: string
  } | null>(null)
  const [recipeResults, setRecipeResults] = useState<RecipeSummaryDto[]>([])
  const [showResults, setShowResults] = useState(false)
  const [scale, setScale] = useState(1)
  const [showNewQueue, setShowNewQueue] = useState(false)
  const [newQueueName, setNewQueueName] = useState('')
  const [deleteQueueId, setDeleteQueueId] = useState<string | null>(null)
  const recipeInputRef = useRef<HTMLInputElement>(null)
  const noteInputRef = useRef<HTMLInputElement>(null)
  const newQueueRef = useRef<HTMLInputElement>(null)
  const resultsRef = useRef<HTMLDivElement>(null)
  const addButtonRef = useRef<HTMLButtonElement>(null)

  const debouncedQuery = useDebouncedValue(recipeQuery, 250)

  useEffect(() => {
    if (addingToQueue) {
      setTimeout(() => {
        if (addMode === 'FreeText') {
          noteInputRef.current?.focus()
        } else {
          recipeInputRef.current?.focus()
        }
      }, 50)
    }
  }, [addingToQueue, addMode])

  useEffect(() => {
    if (showNewQueue && newQueueRef.current) {
      newQueueRef.current.focus()
    }
  }, [showNewQueue])

  // Recipe search — skip when a recipe is already selected
  useEffect(() => {
    if (debouncedQuery.length < 2 || addMode !== 'Recipe' || selectedRecipe) {
      if (!selectedRecipe) setRecipeResults([])
      return
    }
    let cancelled = false
    apiClient
      .get(
        `/api/v1/recipes?filters=title @=* "${debouncedQuery}"&pageSize=500`,
      )
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
  }, [debouncedQuery, addMode, selectedRecipe])

  // Click outside to close results
  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (
        resultsRef.current &&
        !resultsRef.current.contains(e.target as Node) &&
        recipeInputRef.current &&
        !recipeInputRef.current.contains(e.target as Node)
      ) {
        setShowResults(false)
      }
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  function resetAddState() {
    setAddingToQueue(null)
    setAddMode('Recipe')
    setNoteTitle('')
    setRecipeQuery('')
    setSelectedRecipe(null)
    setRecipeResults([])
    setShowResults(false)
    setScale(1)
  }

  function handleAddItem(queueId: string) {
    if (addMode === 'FreeText') {
      if (!noteTitle.trim()) return
      addQueueItem.mutate(
        {
          queueId,
          dto: { recipeId: null, title: noteTitle.trim(), scale: 1 },
        },
        { onSuccess: resetAddState },
      )
    } else {
      const title = selectedRecipe?.title || recipeQuery.trim()
      if (!title) return
      addQueueItem.mutate(
        {
          queueId,
          dto: {
            recipeId: selectedRecipe?.id ?? null,
            title,
            scale,
          },
        },
        { onSuccess: resetAddState },
      )
    }
  }

  function handleDeleteItem(queueId: string, itemId: string) {
    deleteQueueItem.mutate({ queueId, itemId })
  }

  function handleCreateQueue() {
    if (!newQueueName.trim()) return
    addQueue.mutate(
      { name: newQueueName.trim() },
      {
        onSuccess: () => {
          setNewQueueName('')
          setShowNewQueue(false)
        },
      },
    )
  }

  const canAdd =
    addMode === 'FreeText'
      ? noteTitle.trim().length > 0
      : (selectedRecipe?.title || recipeQuery.trim()).length > 0

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between">
        <h3 className="text-sm font-semibold">Queue</h3>
        <Button
          variant="ghost"
          size="icon"
          className="h-6 w-6"
          onClick={() => setShowNewQueue(true)}
        >
          <HugeiconsIcon icon={Add01Icon} className="size-3.5" />
        </Button>
      </div>

      {showNewQueue && (
        <div className="flex gap-1 max-w-xs">
          <Input
            ref={newQueueRef}
            placeholder="Queue name"
            value={newQueueName}
            onChange={(e) => setNewQueueName(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') handleCreateQueue()
              if (e.key === 'Escape') {
                setShowNewQueue(false)
                setNewQueueName('')
              }
            }}
            className="h-7 text-xs"
          />
          <Button
            size="sm"
            className="h-7 px-2"
            onClick={handleCreateQueue}
            disabled={!newQueueName.trim()}
          >
            Add
          </Button>
        </div>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
      {queues.map((queue) => (
        <Collapsible key={queue.id} defaultOpen className="group/queue">
          <div className="flex items-center gap-1">
            <CollapsibleTrigger className="flex flex-1 items-center gap-1 rounded px-1 py-0.5 text-xs font-medium hover:bg-accent">
              <HugeiconsIcon
                icon={ArrowRight01Icon}
                className="size-3 transition-transform group-data-[open]/queue:rotate-90"
              />
              <span className="flex-1 text-left">{queue.name}</span>
              <span className="text-muted-foreground">
                {queue.items.length}
              </span>
            </CollapsibleTrigger>
            {!queue.isDefault && (
              <Button
                variant="ghost"
                size="icon"
                className="h-5 w-5 opacity-0 group-hover/queue:opacity-100 text-destructive hover:text-destructive"
                onClick={() => setDeleteQueueId(queue.id)}
              >
                <HugeiconsIcon icon={Delete01Icon} className="size-3" />
              </Button>
            )}
          </div>
          <CollapsibleContent>
            <div className="space-y-2 mt-1.5 pl-4">
              {queue.items.map((item) => (
                <MealPlanQueueItem
                  key={item.id}
                  item={item}
                  onDelete={(itemId) => handleDeleteItem(queue.id, itemId)}
                />
              ))}
              {addingToQueue === queue.id ? (
                <div className="space-y-2 rounded-md border p-2">
                  <div className="flex rounded-lg border bg-muted p-0.5">
                    <Button
                      variant={addMode === 'Recipe' ? 'default' : 'ghost'}
                      size="sm"
                      className="flex-1 h-6 text-xs"
                      onClick={() => setAddMode('Recipe')}
                    >
                      Recipe
                    </Button>
                    <Button
                      variant={addMode === 'FreeText' ? 'default' : 'ghost'}
                      size="sm"
                      className="flex-1 h-6 text-xs"
                      onClick={() => setAddMode('FreeText')}
                    >
                      Note
                    </Button>
                  </div>

                  {addMode === 'Recipe' ? (
                    <div className="space-y-1.5">
                      <div className="relative">
                        <Input
                          ref={recipeInputRef}
                          placeholder="Search recipes..."
                          value={recipeQuery}
                          onChange={(e) => {
                            setRecipeQuery(e.target.value)
                            setSelectedRecipe(null)
                            setShowResults(true)
                          }}
                          onFocus={() => {
                            if (recipeResults.length > 0) setShowResults(true)
                          }}
                          onKeyDown={(e) => {
                            if (e.key === 'Enter' && canAdd)
                              handleAddItem(queue.id)
                            if (e.key === 'Escape') resetAddState()
                          }}
                          className="h-7 text-xs"
                        />
                        {showResults && recipeResults.length > 0 && (
                          <div
                            ref={resultsRef}
                            className="absolute top-full left-0 right-0 z-50 mt-1 max-h-56 overflow-y-auto rounded-md border bg-popover shadow-md"
                          >
                            {recipeResults.map((recipe) => (
                              <button
                                key={recipe.id}
                                type="button"
                                className="flex w-full items-center gap-2.5 px-2 py-2 text-sm hover:bg-accent text-left"
                                onClick={() => {
                                  setSelectedRecipe({
                                    id: recipe.id,
                                    title: recipe.title,
                                  })
                                  setRecipeQuery(recipe.title)
                                  setShowResults(false)
                                  recipeInputRef.current?.blur()
                                  setTimeout(() => addButtonRef.current?.focus(), 0)
                                }}
                              >
                                {recipe.imageUrl ? (
                                  <img
                                    src={recipe.imageUrl}
                                    alt=""
                                    className="size-7 shrink-0 rounded object-cover"
                                  />
                                ) : (
                                  <div className="size-7 shrink-0 rounded bg-muted" />
                                )}
                                <span className="truncate">
                                  {recipe.title}
                                </span>
                              </button>
                            ))}
                          </div>
                        )}
                      </div>
                      {selectedRecipe && (
                        <p className="text-xs text-muted-foreground truncate">
                          Selected: {selectedRecipe.title}
                        </p>
                      )}
                      <div className="flex items-center gap-1.5">
                        <span className="text-xs text-muted-foreground">Scale:</span>
                        <Input
                          type="number"
                          min={0.25}
                          step={0.25}
                          value={scale}
                          onChange={(e) => {
                            const v = parseFloat(e.target.value)
                            if (!isNaN(v) && v > 0) setScale(v)
                          }}
                          className="h-7 w-16 text-xs"
                        />
                      </div>
                    </div>
                  ) : (
                    <Input
                      ref={noteInputRef}
                      placeholder="Note title..."
                      value={noteTitle}
                      onChange={(e) => setNoteTitle(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === 'Enter' && canAdd)
                          handleAddItem(queue.id)
                        if (e.key === 'Escape') resetAddState()
                      }}
                      className="h-7 text-xs"
                    />
                  )}

                  <div className="flex gap-1">
                    <Button
                      ref={addButtonRef}
                      size="sm"
                      className="h-7 px-2 flex-1"
                      onClick={() => handleAddItem(queue.id)}
                      disabled={!canAdd}
                    >
                      Add
                    </Button>
                    <Button
                      variant="outline"
                      size="sm"
                      className="h-7 px-2"
                      onClick={resetAddState}
                    >
                      Cancel
                    </Button>
                  </div>
                </div>
              ) : (
                <button
                  type="button"
                  onClick={() => {
                    setAddingToQueue(queue.id)
                    setAddMode('Recipe')
                    setNoteTitle('')
                    setRecipeQuery('')
                    setSelectedRecipe(null)
                    setScale(1)
                  }}
                  className="flex items-center gap-1 rounded px-1.5 py-1 text-xs text-muted-foreground hover:text-foreground hover:bg-accent w-full"
                >
                  <HugeiconsIcon icon={Add01Icon} className="size-3" />
                  Add item
                </button>
              )}
            </div>
          </CollapsibleContent>
        </Collapsible>
      ))}
      </div>

      <AlertDialog
        open={deleteQueueId !== null}
        onOpenChange={(open) => {
          if (!open) setDeleteQueueId(null)
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Queue</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &ldquo;
              {queues.find((q) => q.id === deleteQueueId)?.name}
              &rdquo;? All items in this queue will be removed.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={() => {
                if (deleteQueueId) deleteQueue.mutate(deleteQueueId)
                setDeleteQueueId(null)
              }}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
