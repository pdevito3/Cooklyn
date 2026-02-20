import { useRef, useState, useEffect, useMemo, useCallback } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useHotkeys } from 'react-hotkeys-hook'
import {
  Add01Icon,
  RotateClockwiseIcon,
  Search01Icon,
  Loading03Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { useQueryClient } from '@tanstack/react-query'
import { AnimatePresence, motion } from 'motion/react'

import { useInfiniteRecipes } from '@/domain/recipes/apis/get-recipes'
import { RecipeKeys } from '@/domain/recipes/apis/recipe.keys'
import { useDeleteRecipe } from '@/domain/recipes/apis/recipe-mutations'
import {
  transformCollectionFilters,
  FLAG_FILTER_OPTIONS,
  RATING_FILTER_OPTIONS,
} from '@/domain/recipes/utils/recipe-filter-helpers'
import { useSavedFilters } from '@/domain/saved-filters/apis/get-saved-filters'
import {
  useCreateSavedFilter,
  useDeleteSavedFilter as useDeleteSavedFilterMutation,
  useUpdateSavedFilter,
} from '@/domain/saved-filters/apis/saved-filter-mutations'
import {
  serializeFilterState,
  deserializeFilterState,
} from '@/domain/saved-filters/utils/filter-state-serialization'
import { useTags } from '@/domain/tags/apis/get-tags'
import { RecipeCard } from '@/components/recipe-card'
import { RecipeSmallCard } from '@/components/recipe-small-card'
import { RecipeListItem } from '@/components/recipe-list-item'
import {
  RecipeViewToggle,
  type RecipeViewMode,
} from '@/components/recipe-view-toggle'
import {
  FilterBuilder,
  toQueryKitString,
} from '@/components/filter-builder/filter-builder'
import type {
  FilterConfig,
  FilterPreset,
  FilterState,
  SavedFilterItem,
} from '@/components/filter-builder/types'
import { Operators } from '@/components/filter-builder/utils/operators'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Kbd } from '@/components/ui/kbd'
import { Skeleton } from '@/components/ui/skeleton'
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
import { AddRecipeToShoppingListDialog } from '@/components/add-recipe-to-shopping-list-dialog'
import { AddToMealPlanPopover } from '@/components/meal-plan/add-to-meal-plan-popover'
import { useDebouncedValue } from '@/hooks/use-debounced-value'

const PAGE_SIZE = 24

export const Route = createFileRoute('/recipes/')({
  component: RecipesIndexPage,
})

function RecipesIndexPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [viewMode, setViewMode] = useState<RecipeViewMode>(() => {
    const stored = localStorage.getItem('recipe-view-mode')
    if (stored === 'cards' || stored === 'small-cards' || stored === 'list')
      return stored
    return 'cards'
  })
  const [searchQuery, setSearchQuery] = useState('')
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [recipeToDelete, setRecipeToDelete] = useState<string | null>(null)
  const [addToListDialogOpen, setAddToListDialogOpen] = useState(false)
  const [recipeForList, setRecipeForList] = useState<string | null>(null)
  const [mealPlanDialogOpen, setMealPlanDialogOpen] = useState(false)
  const [recipeForMealPlan, setRecipeForMealPlan] = useState<string | null>(null)
  const [filterBuilderState, setFilterBuilderState] = useState<FilterState>({
    filters: [],
    rootLogicalOperator: 'AND',
  })
  const [filterBuilderKey, setFilterBuilderKey] = useState(0)
  const searchInputRef = useRef<HTMLInputElement>(null)
  const sentinelRef = useRef<HTMLDivElement>(null)

  const debouncedSearch = useDebouncedValue(searchQuery, 300)

  // Fetch saved filters for recipes context
  const { data: savedFiltersData } = useSavedFilters('recipes')
  const createSavedFilter = useCreateSavedFilter()
  const deleteSavedFilterMutation = useDeleteSavedFilterMutation()
  const updateSavedFilterMutation = useUpdateSavedFilter()

  const savedFilterItems: SavedFilterItem[] = useMemo(
    () =>
      savedFiltersData?.items.map((sf) => ({
        id: sf.id,
        name: sf.name,
        filterState: deserializeFilterState(sf.filterStateJson),
      })) ?? [],
    [savedFiltersData],
  )

  const handleSaveFilter = useCallback(
    (name: string) => {
      createSavedFilter.mutate({
        name,
        context: 'recipes',
        filterStateJson: serializeFilterState(filterBuilderState),
      })
    },
    [createSavedFilter, filterBuilderState],
  )

  const handleDeleteSavedFilter = useCallback(
    (id: string) => {
      deleteSavedFilterMutation.mutate(id)
    },
    [deleteSavedFilterMutation],
  )

  const handleUpdateSavedFilter = useCallback(
    (id: string) => {
      updateSavedFilterMutation.mutate({
        id,
        dto: {
          name:
            savedFilterItems.find((sf) => sf.id === id)?.name ?? 'Untitled',
          filterStateJson: serializeFilterState(filterBuilderState),
        },
      })
    },
    [updateSavedFilterMutation, savedFilterItems, filterBuilderState],
  )

  // Fetch tags for filter options
  const { data: tagsData } = useTags({ pageSize: 200, sortOrder: 'Name' })

  const tagOptions = useMemo(
    () =>
      tagsData?.items.map((t) => ({ value: t.name, label: t.name })) ?? [],
    [tagsData],
  )

  // Build filter configuration
  const filterOptions: FilterConfig[] = useMemo(
    () => [
      {
        propertyKey: 'title',
        propertyLabel: 'Title',
        controlType: 'text',
      },
      {
        propertyKey: 'description',
        propertyLabel: 'Description',
        controlType: 'text',
      },
      {
        propertyKey: 'rating',
        propertyLabel: 'Rating',
        controlType: 'multiselect',
        options: RATING_FILTER_OPTIONS,
      },
      {
        propertyKey: 'flags',
        propertyLabel: 'Dietary Flags',
        controlType: 'multiselect',
        options: FLAG_FILTER_OPTIONS,
      },
      {
        propertyKey: 'tags',
        propertyLabel: 'Tags',
        controlType: 'multiselect',
        options: tagOptions,
      },
      {
        propertyKey: 'Servings',
        propertyLabel: 'Servings',
        controlType: 'number',
      },
      {
        propertyKey: 'Ingredients',
        propertyLabel: 'Ingredient Count',
        controlType: 'number',
        operators: [
          Operators.COUNT_EQUALS,
          Operators.COUNT_NOT_EQUALS,
          Operators.COUNT_GREATER_THAN,
          Operators.COUNT_LESS_THAN,
          Operators.COUNT_GREATER_THAN_OR_EQUAL,
          Operators.COUNT_LESS_THAN_OR_EQUAL,
        ],
        defaultOperator: Operators.COUNT_GREATER_THAN_OR_EQUAL,
      },
      {
        propertyKey: 'CreatedOn',
        propertyLabel: 'Created Date',
        controlType: 'date',
        dateType: 'datetimeOffset',
      },
    ],
    [tagOptions],
  )

  // Build filter presets
  const presets: FilterPreset[] = useMemo(
    () => [
      {
        label: 'Quick & Easy',
        filter: {
          filters: [
            {
              id: crypto.randomUUID(),
              propertyKey: 'flags',
              propertyLabel: 'Dietary Flags',
              controlType: 'multiselect',
              operator: Operators.IN,
              value: ['QuickAndEasy'],
              selectedLabels: ['Quick & Easy'],
            },
          ],
          rootLogicalOperator: 'AND',
        },
      },
      {
        label: 'Top Rated',
        filter: {
          filters: [
            {
              id: crypto.randomUUID(),
              propertyKey: 'rating',
              propertyLabel: 'Rating',
              controlType: 'multiselect',
              operator: Operators.IN,
              value: ['Loved It', 'Liked It'],
              selectedLabels: ['Loved It', 'Liked It'],
            },
          ],
          rootLogicalOperator: 'AND',
        },
      },
      {
        label: 'Recently Added',
        filter: {
          filters: [
            {
              id: crypto.randomUUID(),
              propertyKey: 'CreatedOn',
              propertyLabel: 'Created Date',
              controlType: 'date',
              operator: Operators.GREATER_THAN,
              value: {
                mode: 'after' as const,
                startDate: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
                dateType: 'datetimeOffset' as const,
              },
            },
          ],
          rootLogicalOperator: 'AND',
        },
      },
      {
        label: 'Vegan & Gluten-Free',
        filter: {
          filters: [
            {
              id: crypto.randomUUID(),
              propertyKey: 'flags',
              propertyLabel: 'Dietary Flags',
              controlType: 'multiselect',
              operator: Operators.IN,
              value: ['Vegan', 'GlutenFree'],
              selectedLabels: ['Vegan', 'Gluten-Free'],
              matchAll: true,
            },
          ],
          rootLogicalOperator: 'AND',
        },
      },
    ],
    [],
  )

  // Convert filter builder state to QueryKit string with collection post-processing
  const advancedFilterString = useMemo(() => {
    const raw = toQueryKitString(filterBuilderState)
    if (!raw) return ''
    return transformCollectionFilters(raw)
  }, [filterBuilderState])

  // Combine search bar + filter builder into unified filter string
  const filters = useMemo(() => {
    const parts: string[] = []
    if (debouncedSearch) {
      parts.push(`title @=* "${debouncedSearch}"`)
    }
    if (advancedFilterString) {
      parts.push(advancedFilterString)
    }
    return parts.length > 0 ? parts.join(' && ') : undefined
  }, [debouncedSearch, advancedFilterString])

  const hasActiveFilters =
    searchQuery.length > 0 || filterBuilderState.filters.length > 0

  const {
    data,
    isLoading,
    error,
    isFetching,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
  } = useInfiniteRecipes({ pageSize: PAGE_SIZE, filters })

  const deleteRecipe = useDeleteRecipe()

  const allRecipes = useMemo(
    () => data?.pages.flatMap((p) => p.items) ?? [],
    [data],
  )

  const totalCount = data?.pages[0]?.pagination.totalCount ?? 0

  // Infinite scroll via IntersectionObserver on sentinel element
  useEffect(() => {
    const el = sentinelRef.current
    if (!el) return

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting && hasNextPage && !isFetchingNextPage) {
          fetchNextPage()
        }
      },
      { rootMargin: '300px' },
    )
    observer.observe(el)
    return () => observer.disconnect()
  }, [hasNextPage, isFetchingNextPage, fetchNextPage])

  const handleViewModeChange = useCallback((mode: RecipeViewMode) => {
    setViewMode(mode)
    localStorage.setItem('recipe-view-mode', mode)
  }, [])

  const handleRefresh = () => {
    queryClient.invalidateQueries({ queryKey: RecipeKeys.all })
  }

  const handleCreateRecipe = () => {
    navigate({ to: '/recipes/new' })
  }

  const handleFilterChange = useCallback((state: FilterState) => {
    setFilterBuilderState(state)
  }, [])

  const handleClearAll = () => {
    setSearchQuery('')
    setFilterBuilderState({ filters: [], rootLogicalOperator: 'AND' })
    setFilterBuilderKey((prev) => prev + 1)
  }

  useHotkeys(
    'mod+f',
    () => {
      searchInputRef.current?.focus()
    },
    { preventDefault: true },
  )
  useHotkeys('c', () => {
    handleCreateRecipe()
  })

  const handleEditRecipe = useCallback(
    (id: string) => {
      navigate({ to: '/recipes/$id/edit', params: { id } })
    },
    [navigate],
  )

  const handleDeleteRecipe = useCallback((id: string) => {
    setRecipeToDelete(id)
    setDeleteDialogOpen(true)
  }, [])

  const handleAddToShoppingList = useCallback((id: string) => {
    setRecipeForList(id)
    setAddToListDialogOpen(true)
  }, [])

  const handleAddToMealPlan = useCallback((id: string) => {
    setRecipeForMealPlan(id)
    setMealPlanDialogOpen(true)
  }, [])

  const confirmDelete = () => {
    if (recipeToDelete) {
      deleteRecipe.mutate(recipeToDelete, {
        onSuccess: () => {
          setDeleteDialogOpen(false)
          setRecipeToDelete(null)
        },
      })
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Recipes</h1>
          <p className="text-muted-foreground">Manage your recipe collection</p>
        </div>
        <Button onClick={handleCreateRecipe}>
          <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
          New Recipe
          <Kbd>C</Kbd>
        </Button>
      </div>

      {/* Search and Controls */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="relative max-w-sm flex-1">
          <HugeiconsIcon
            icon={Search01Icon}
            className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
          />
          <Input
            ref={searchInputRef}
            placeholder="Search recipes..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="pl-9 pr-16"
            autoFocus
          />
          <div className="pointer-events-none absolute right-2 top-1/2 -translate-y-1/2">
            <Kbd>&#8984;F</Kbd>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {hasActiveFilters && (
            <Button variant="outline" size="sm" onClick={handleClearAll}>
              Clear all
            </Button>
          )}
          <RecipeViewToggle value={viewMode} onChange={handleViewModeChange} />
          <Button
            variant="outline"
            onClick={handleRefresh}
            disabled={isFetching}
          >
            <HugeiconsIcon
              icon={RotateClockwiseIcon}
              className={`mr-2 h-4 w-4 ${isFetching ? 'animate-spin' : ''}`}
            />
            Refresh
          </Button>
        </div>
      </div>

      {/* Advanced Filters */}
      <FilterBuilder
        key={filterBuilderKey}
        filterOptions={filterOptions}
        presets={presets}
        savedFilters={savedFilterItems}
        onSaveFilter={handleSaveFilter}
        onDeleteSavedFilter={handleDeleteSavedFilter}
        onUpdateSavedFilter={handleUpdateSavedFilter}
        onChange={handleFilterChange}
      />

      {/* Error State */}
      {error && (
        <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
          <p className="font-medium">Error loading recipes</p>
          <p className="mt-1 text-sm">
            {error instanceof Error ? error.message : 'Failed to fetch recipes'}
          </p>
        </div>
      )}

      {/* Loading State */}
      {isLoading && (
        <div>
          <div className="grid grid-cols-[repeat(auto-fit,_minmax(250px,_1fr))] gap-6">
            {[...Array(8)].map((_, i) => (
              <div key={i} className="space-y-3">
                <Skeleton className="aspect-video w-full rounded-lg" />
                <Skeleton className="h-6 w-3/4" />
                <Skeleton className="h-4 w-full" />
                <div className="flex gap-2">
                  <Skeleton className="h-5 w-16" />
                  <Skeleton className="h-5 w-16" />
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Recipe Grid / List */}
      {!isLoading && allRecipes.length > 0 && (
        <div>
          <AnimatePresence mode="wait">
            <motion.div
              key={viewMode}
              initial={{ opacity: 0, y: 8 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -8 }}
              transition={{ duration: 0.2, ease: 'easeInOut' }}
            >
              {viewMode === 'cards' && (
                <div className="grid grid-cols-[repeat(auto-fit,minmax(18rem,1fr))] gap-6">
                  {allRecipes.map((recipe) => (
                    <RecipeCard
                      key={recipe.id}
                      recipe={recipe}
                      onEdit={handleEditRecipe}
                      onDelete={handleDeleteRecipe}
                      onAddToShoppingList={handleAddToShoppingList}
                      onAddToMealPlan={handleAddToMealPlan}
                    />
                  ))}
                </div>
              )}

              {viewMode === 'small-cards' && (
                <div className="grid grid-cols-[repeat(auto-fit,minmax(12rem,1fr))] gap-4">
                  {allRecipes.map((recipe) => (
                    <RecipeSmallCard
                      key={recipe.id}
                      recipe={recipe}
                      onEdit={handleEditRecipe}
                      onDelete={handleDeleteRecipe}
                      onAddToShoppingList={handleAddToShoppingList}
                      onAddToMealPlan={handleAddToMealPlan}
                    />
                  ))}
                </div>
              )}

              {viewMode === 'list' && (
                <div className="flex flex-col gap-2">
                  {allRecipes.map((recipe) => (
                    <RecipeListItem
                      key={recipe.id}
                      recipe={recipe}
                      onEdit={handleEditRecipe}
                      onDelete={handleDeleteRecipe}
                      onAddToShoppingList={handleAddToShoppingList}
                      onAddToMealPlan={handleAddToMealPlan}
                    />
                  ))}
                </div>
              )}
            </motion.div>
          </AnimatePresence>

          {/* Infinite scroll sentinel */}
          <div ref={sentinelRef} className="h-1" />

          {/* Loading more indicator */}
          {isFetchingNextPage && (
            <div className="flex items-center justify-center py-6">
              <HugeiconsIcon
                icon={Loading03Icon}
                className="h-6 w-6 animate-spin text-muted-foreground"
              />
              <span className="ml-2 text-sm text-muted-foreground">
                Loading more recipes...
              </span>
            </div>
          )}
        </div>
      )}

      {/* Empty State */}
      {!isLoading && !error && allRecipes.length === 0 && (
        <div className="flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
          {hasActiveFilters ? (
            <>
              <HugeiconsIcon
                icon={Search01Icon}
                className="mb-4 h-12 w-12 text-muted-foreground"
              />
              <p className="text-muted-foreground">
                No recipes match your filters
              </p>
              <Button
                variant="outline"
                className="mt-4"
                onClick={handleClearAll}
              >
                Clear all filters
              </Button>
            </>
          ) : (
            <>
              <p className="text-muted-foreground">No recipes yet</p>
              <Button className="mt-4" onClick={handleCreateRecipe}>
                <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
                Create your first recipe
              </Button>
            </>
          )}
        </div>
      )}

      {/* Pagination Info */}
      {totalCount > 0 && (
        <div className="text-sm text-muted-foreground">
          <p>
            Showing {allRecipes.length} of {totalCount} recipes
          </p>
        </div>
      )}

      {/* Add to Shopping List Dialog */}
      {recipeForList && (
        <AddRecipeToShoppingListDialog
          open={addToListDialogOpen}
          onOpenChange={setAddToListDialogOpen}
          recipeId={recipeForList}
        />
      )}

      {/* Add to Meal Plan Dialog */}
      {recipeForMealPlan && (
        <AddToMealPlanPopover
          open={mealPlanDialogOpen}
          onOpenChange={setMealPlanDialogOpen}
          recipeId={recipeForMealPlan}
          recipeTitle={
            allRecipes.find((r) => r.id === recipeForMealPlan)?.title ?? ''
          }
        />
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Recipe</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete this recipe? This action cannot be
              undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteRecipe.isPending ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
