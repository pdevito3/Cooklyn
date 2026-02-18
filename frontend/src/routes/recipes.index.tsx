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

import { useInfiniteRecipes } from '@/domain/recipes/apis/get-recipes'
import { RecipeKeys } from '@/domain/recipes/apis/recipe.keys'
import { useDeleteRecipe } from '@/domain/recipes/apis/recipe-mutations'
import { RecipeCard } from '@/components/recipe-card'
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
import { useDebouncedValue } from '@/hooks/use-debounced-value'

const PAGE_SIZE = 24

export const Route = createFileRoute('/recipes/')({
  component: RecipesIndexPage,
})

function RecipesIndexPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [searchQuery, setSearchQuery] = useState('')
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [recipeToDelete, setRecipeToDelete] = useState<string | null>(null)
  const [addToListDialogOpen, setAddToListDialogOpen] = useState(false)
  const [recipeForList, setRecipeForList] = useState<string | null>(null)
  const searchInputRef = useRef<HTMLInputElement>(null)
  const sentinelRef = useRef<HTMLDivElement>(null)

  const debouncedSearch = useDebouncedValue(searchQuery, 300)

  const filters = debouncedSearch ? `title @=* "${debouncedSearch}"` : undefined

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

  const handleRefresh = () => {
    queryClient.invalidateQueries({ queryKey: RecipeKeys.all })
  }

  const handleCreateRecipe = () => {
    navigate({ to: '/recipes/new' })
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
            className="pl-9"
            autoFocus
          />
        </div>
        <Button variant="outline" onClick={handleRefresh} disabled={isFetching}>
          <HugeiconsIcon
            icon={RotateClockwiseIcon}
            className={`mr-2 h-4 w-4 ${isFetching ? 'animate-spin' : ''}`}
          />
          Refresh
        </Button>
      </div>

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

      {/* Recipe Grid */}
      {!isLoading && allRecipes.length > 0 && (
        <div>
          <div className="grid grid-cols-[repeat(auto-fit,minmax(18rem,1fr))] gap-6">
            {allRecipes.map((recipe) => (
              <RecipeCard
                key={recipe.id}
                recipe={recipe}
                onEdit={handleEditRecipe}
                onDelete={handleDeleteRecipe}
                onAddToShoppingList={handleAddToShoppingList}
              />
            ))}
          </div>

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
          {searchQuery ? (
            <>
              <HugeiconsIcon
                icon={Search01Icon}
                className="mb-4 h-12 w-12 text-muted-foreground"
              />
              <p className="text-muted-foreground">
                No recipes match your search
              </p>
              <Button
                variant="outline"
                className="mt-4"
                onClick={() => setSearchQuery('')}
              >
                Clear search
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
