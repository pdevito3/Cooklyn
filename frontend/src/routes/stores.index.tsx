import { useState } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { Add01Icon, Store01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import { useStores, useCreateStore, useDeleteStore } from '@/domain/stores'
import { useMyDefaultStore } from '@/domain/users'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
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

export const Route = createFileRoute('/stores/')({
  component: StoresIndexPage,
})

function StoresIndexPage() {
  const navigate = useNavigate()
  const { data, isLoading, error } = useStores({ pageSize: 100 })
  const { data: defaultStoreId } = useMyDefaultStore()
  const createStore = useCreateStore()
  const deleteStoreMutation = useDeleteStore()
  const [createOpen, setCreateOpen] = useState(false)
  const [deleteId, setDeleteId] = useState<string | null>(null)
  const [newName, setNewName] = useState('')
  const [newAddress, setNewAddress] = useState('')

  const handleCreate = () => {
    createStore.mutate(
      { name: newName, address: newAddress || null },
      {
        onSuccess: (store) => {
          setCreateOpen(false)
          setNewName('')
          setNewAddress('')
          navigate({ to: '/stores/$id', params: { id: store.id } })
        },
      }
    )
  }

  const confirmDelete = () => {
    if (deleteId) {
      deleteStoreMutation.mutate(deleteId, {
        onSuccess: () => setDeleteId(null),
      })
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Stores</h1>
          <p className="text-muted-foreground">Manage your stores and aisle ordering</p>
        </div>
        <Button onClick={() => setCreateOpen(true)}>
          <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
          New Store
        </Button>
      </div>

      {error && (
        <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
          <p className="font-medium">Error loading stores</p>
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
          {data.items.map((store) => (
            <Card
              key={store.id}
              className="cursor-pointer transition-shadow hover:shadow-md"
              onClick={() => navigate({ to: '/stores/$id', params: { id: store.id } })}
            >
              <CardHeader className="flex flex-row items-center gap-3">
                <HugeiconsIcon icon={Store01Icon} className="h-5 w-5 text-muted-foreground" />
                <CardTitle className="text-lg">{store.name}</CardTitle>
                {defaultStoreId === store.id && <Badge variant="secondary">Default</Badge>}
              </CardHeader>
              <CardContent>
                {store.address && (
                  <p className="text-sm text-muted-foreground">{store.address}</p>
                )}
                <p className="text-sm text-muted-foreground mt-1">
                  {store.storeAisles.length} aisle{store.storeAisles.length !== 1 ? 's' : ''}
                </p>
              </CardContent>
            </Card>
          ))}

          {data.items.length === 0 && (
            <div className="col-span-full flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
              <p className="text-muted-foreground">No stores yet</p>
              <Button className="mt-4" onClick={() => setCreateOpen(true)}>
                <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
                Create your first store
              </Button>
            </div>
          )}
        </div>
      )}

      <Dialog open={createOpen} onOpenChange={setCreateOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>New Store</DialogTitle>
          </DialogHeader>
          <form onSubmit={(e) => { e.preventDefault(); handleCreate() }}>
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="store-name">Name</Label>
                <Input
                  id="store-name"
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  placeholder="e.g. Tribble Publix"
                  autoFocus
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="store-address">Address (optional)</Label>
                <Input
                  id="store-address"
                  value={newAddress}
                  onChange={(e) => setNewAddress(e.target.value)}
                  placeholder="e.g. 123 Main St"
                />
              </div>
            </div>
            <DialogFooter className="mt-4">
              <Button type="button" variant="outline" onClick={() => setCreateOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={!newName.trim() || createStore.isPending}>
                {createStore.isPending ? 'Creating...' : 'Create'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <AlertDialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Store</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete this store? This action cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteStoreMutation.isPending ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
