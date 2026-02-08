import { useState } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { Add01Icon, RotateClockwiseIcon, Search01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { useQueryClient } from '@tanstack/react-query'

import { useRecipes, RecipeKeys, useDeleteRecipe, useToggleRecipeFavorite } from '@/domain/recipes'
import { RecipeCard } from '@/components/recipe-card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
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

export const Route = createFileRoute('/recipes/')({
  component: RecipesIndexPage,
})

function RecipesIndexPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [searchQuery, setSearchQuery] = useState('')
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [recipeToDelete, setRecipeToDelete] = useState<string | null>(null)

  const { data, isLoading, error, isFetching } = useRecipes()
  const deleteRecipe = useDeleteRecipe()
  const toggleFavorite = useToggleRecipeFavorite()

  const handleRefresh = () => {
    queryClient.invalidateQueries({ queryKey: RecipeKeys.all })
  }

  const handleCreateRecipe = () => {
    navigate({ to: '/recipes/new' })
  }

  const handleEditRecipe = (id: string) => {
    navigate({ to: '/recipes/$id/edit', params: { id } })
  }

  const handleDeleteRecipe = (id: string) => {
    setRecipeToDelete(id)
    setDeleteDialogOpen(true)
  }

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

  const handleToggleFavorite = (id: string) => {
    toggleFavorite.mutate(id)
  }

  const filteredRecipes = data?.items.filter((recipe) =>
    recipe.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
    recipe.description?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    recipe.tags.some((tag) => tag.toLowerCase().includes(searchQuery.toLowerCase()))
  ) ?? []

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Recipes</h1>
          <p className="text-muted-foreground">
            Manage your recipe collection
          </p>
        </div>
        <Button onClick={handleCreateRecipe}>
          <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
          New Recipe
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
            placeholder="Search recipes..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="pl-9"
            autoFocus
          />
        </div>
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
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
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
      )}

      {/* Recipe Grid */}
      {!isLoading && filteredRecipes.length > 0 && (
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          {filteredRecipes.map((recipe) => (
            <RecipeCard
              key={recipe.id}
              recipe={recipe}
              onEdit={handleEditRecipe}
              onDelete={handleDeleteRecipe}
              onToggleFavorite={handleToggleFavorite}
            />
          ))}
        </div>
      )}

      {/* Empty State */}
      {!isLoading && !error && filteredRecipes.length === 0 && (
        <div className="flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
          {searchQuery ? (
            <>
              <HugeiconsIcon
                icon={Search01Icon}
                className="mb-4 h-12 w-12 text-muted-foreground"
              />
              <p className="text-muted-foreground">No recipes match your search</p>
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
      {data && data.pagination.totalCount > 0 && (
        <div className="flex items-center justify-between text-sm text-muted-foreground">
          <p>
            Showing {filteredRecipes.length} of {data.pagination.totalCount} recipes
          </p>
          {data.pagination.totalPages > 1 && (
            <p>
              Page {data.pagination.currentPage} of {data.pagination.totalPages}
            </p>
          )}
        </div>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Recipe</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete this recipe? This action cannot be undone.
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
