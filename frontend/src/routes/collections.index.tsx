import { useState } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { Add01Icon, Layers01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import {
  useItemCollections,
  useCreateItemCollection,
  useDeleteItemCollection,
} from '@/domain/item-collections'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
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
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

export const Route = createFileRoute('/collections/')({
  component: CollectionsIndexPage,
})

function CollectionsIndexPage() {
  const navigate = useNavigate()
  const { data, isLoading, error } = useItemCollections({ pageSize: 100 })
  const createCollection = useCreateItemCollection()
  const deleteCollectionMutation = useDeleteItemCollection()
  const [createOpen, setCreateOpen] = useState(false)
  const [deleteId, setDeleteId] = useState<string | null>(null)
  const [newName, setNewName] = useState('')

  const handleCreate = () => {
    createCollection.mutate(
      { name: newName },
      {
        onSuccess: (collection) => {
          setCreateOpen(false)
          setNewName('')
          navigate({ to: '/collections/$id', params: { id: collection.id } })
        },
      }
    )
  }

  const confirmDelete = () => {
    if (deleteId) {
      deleteCollectionMutation.mutate(deleteId, {
        onSuccess: () => setDeleteId(null),
      })
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Collections</h1>
          <p className="text-muted-foreground">Reusable item groups for quick list building</p>
        </div>
        <Button onClick={() => setCreateOpen(true)}>
          <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
          New Collection
        </Button>
      </div>

      {error && (
        <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
          <p className="font-medium">Error loading collections</p>
        </div>
      )}

      {isLoading && (
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {[...Array(3)].map((_, i) => (
            <Skeleton key={i} className="h-32 w-full rounded-lg" />
          ))}
        </div>
      )}

      {!isLoading && data && (
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {data.items.map((collection) => (
            <Card
              key={collection.id}
              className="cursor-pointer transition-shadow hover:shadow-md"
              onClick={() => navigate({ to: '/collections/$id', params: { id: collection.id } })}
            >
              <CardHeader className="flex flex-row items-center gap-3">
                <HugeiconsIcon icon={Layers01Icon} className="h-5 w-5 text-muted-foreground" />
                <CardTitle className="text-lg">{collection.name}</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-sm text-muted-foreground">
                  {collection.items.length} item{collection.items.length !== 1 ? 's' : ''}
                </p>
              </CardContent>
            </Card>
          ))}

          {data.items.length === 0 && (
            <div className="col-span-full flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
              <p className="text-muted-foreground">No collections yet</p>
              <Button className="mt-4" onClick={() => setCreateOpen(true)}>
                <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
                Create your first collection
              </Button>
            </div>
          )}
        </div>
      )}

      <Dialog open={createOpen} onOpenChange={setCreateOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>New Collection</DialogTitle>
          </DialogHeader>
          <form onSubmit={(e) => { e.preventDefault(); handleCreate() }}>
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="collection-name">Name</Label>
                <Input
                  id="collection-name"
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  placeholder="e.g. Weekly Staples"
                  autoFocus
                />
              </div>
            </div>
            <DialogFooter className="mt-4">
              <Button type="button" variant="outline" onClick={() => setCreateOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={!newName.trim() || createCollection.isPending}>
                {createCollection.isPending ? 'Creating...' : 'Create'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <AlertDialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Collection</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete this collection? This action cannot be undone.
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
