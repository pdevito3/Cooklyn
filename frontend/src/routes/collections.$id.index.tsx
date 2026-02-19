import { ArrowLeft02Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useEffect, useState } from 'react'
import { useHotkeys } from 'react-hotkeys-hook'

import { IngredientEditor } from '@/components/ingredient-editor'
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
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Kbd } from '@/components/ui/kbd'
import { Label } from '@/components/ui/label'
import { Skeleton } from '@/components/ui/skeleton'
import { useItemCollection } from '@/domain/item-collections/apis/get-item-collection'
import {
  useDeleteItemCollection,
  useUpdateItemCollection,
  useUpdateItemCollectionItems,
} from '@/domain/item-collections/apis/item-collection-mutations'
import { collectionItemsToIngredients } from '@/domain/item-collections/utils/collection-item-to-ingredient'
import { ingredientsToCollectionItems } from '@/domain/item-collections/utils/ingredient-to-collection-item'
import type { IngredientForCreationDto } from '@/domain/recipes/types'

export const Route = createFileRoute('/collections/$id/')({
  component: CollectionDetailPage,
})

function CollectionDetailPage() {
  const { id } = Route.useParams()
  const navigate = useNavigate()
  const { data: collection, isLoading } = useItemCollection(id)
  const updateCollection = useUpdateItemCollection()
  const deleteCollectionMutation = useDeleteItemCollection()
  const updateItems = useUpdateItemCollectionItems()

  const [deleteOpen, setDeleteOpen] = useState(false)
  const [editName, setEditName] = useState('')
  const [isEditingName, setIsEditingName] = useState(false)
  const [ingredients, setIngredients] = useState<IngredientForCreationDto[]>([])
  const [itemsSynced, setItemsSynced] = useState(false)

  useHotkeys('e', () => {
    if (collection && !isEditingName) startEditingName()
  })
  useHotkeys('delete', () => {
    if (collection && !isEditingName) setDeleteOpen(true)
  })
  useHotkeys(
    'mod+enter',
    () => {
      if (collection && itemsSynced) saveItems()
    },
    { enableOnFormTags: ['INPUT', 'TEXTAREA', 'SELECT'], preventDefault: true },
  )

  // Sync items from collection data when it loads
  useEffect(() => {
    if (collection && !itemsSynced) {
      setIngredients(collectionItemsToIngredients(collection.items))
      setItemsSynced(true)
    }
  }, [collection, itemsSynced])

  const startEditingName = () => {
    if (!collection) return
    setEditName(collection.name)
    setIsEditingName(true)
  }

  const saveNameEdit = () => {
    updateCollection.mutate(
      { id, dto: { name: editName } },
      { onSuccess: () => setIsEditingName(false) },
    )
  }

  const saveItems = () => {
    const items = ingredientsToCollectionItems(ingredients)
    updateItems.mutate({ id, items })
  }

  const confirmDelete = () => {
    deleteCollectionMutation.mutate(id, {
      onSuccess: () => navigate({ to: '/collections' }),
    })
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
      </div>
    )
  }

  if (!collection) {
    return <p className="text-muted-foreground">Collection not found.</p>
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => navigate({ to: '/collections' })}
        >
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-4 w-4" />
        </Button>
        <div className="flex-1">
          <h1 className="text-3xl font-bold tracking-tight">
            {collection.name}
          </h1>
          <p className="text-muted-foreground">
            {collection.items.length} item
            {collection.items.length !== 1 ? 's' : ''}
          </p>
        </div>
        <Button variant="outline" onClick={startEditingName}>
          Edit
          <Kbd>E</Kbd>
        </Button>
        <Button variant="destructive" onClick={() => setDeleteOpen(true)}>
          Delete
          <Kbd>⌫</Kbd>
        </Button>
      </div>

      {isEditingName && (
        <Card>
          <CardHeader>
            <CardTitle>Edit Collection</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label>Name</Label>
              <Input
                value={editName}
                onChange={(e) => setEditName(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) {
                    e.preventDefault()
                    saveNameEdit()
                  }
                }}
              />
            </div>
            <div className="flex gap-2">
              <Button
                onClick={saveNameEdit}
                disabled={updateCollection.isPending}
              >
                Save
                <Kbd>⌘↵</Kbd>
              </Button>
              <Button variant="outline" onClick={() => setIsEditingName(false)}>
                Cancel
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      <Card>
        <CardHeader>
          <CardTitle>Items</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          {itemsSynced && (
            <IngredientEditor value={ingredients} onChange={setIngredients} />
          )}
          <Button onClick={saveItems} disabled={updateItems.isPending}>
            {updateItems.isPending ? 'Saving...' : 'Save'}
            <Kbd>⌘↵</Kbd>
          </Button>
        </CardContent>
      </Card>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Collection</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete "{collection.name}"? This action
              cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteCollectionMutation.isPending ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
