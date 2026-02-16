import { useState } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useHotkeys } from 'react-hotkeys-hook'
import { Add01Icon, ShoppingCart01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { format } from 'date-fns'

import { useShoppingLists } from '@/domain/shopping-lists/apis/get-shopping-lists'
import { useCreateShoppingList, useDeleteShoppingList } from '@/domain/shopping-lists/apis/shopping-list-mutations'
import { useStores } from '@/domain/stores/apis/get-stores'
import { useMyDefaultStore } from '@/domain/users/apis/get-my-default-store'
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
import {
  Combobox,
  ComboboxInput,
  ComboboxContent,
  ComboboxItem,
  ComboboxItemIndicator,
} from '@/components/ui/combobox'
import type { StoreDto } from '@/domain/stores/types'
import { Input } from '@/components/ui/input'
import { Kbd } from '@/components/ui/kbd'
import { Label } from '@/components/ui/label'

export const Route = createFileRoute('/shopping-lists/')({
  component: ShoppingListsIndexPage,
})

function ShoppingListsIndexPage() {
  const navigate = useNavigate()
  const { data, isLoading, error } = useShoppingLists({ pageSize: 100 })
  const { data: storesData } = useStores({ pageSize: 100 })
  const { data: defaultStoreId } = useMyDefaultStore()
  const createList = useCreateShoppingList()
  const deleteListMutation = useDeleteShoppingList()
  const [createOpen, setCreateOpen] = useState(false)
  const [deleteId, setDeleteId] = useState<string | null>(null)
  const [newName, setNewName] = useState('')
  const [newStoreId, setNewStoreId] = useState<string | null>(null)

  const stores = storesData?.items ?? []

  const openCreateDialog = () => {
    setNewName(`Groceries - ${format(new Date(), 'MM/dd/yyyy')}`)
    setNewStoreId(defaultStoreId ?? null)
    setCreateOpen(true)
  }

  useHotkeys('c', () => { openCreateDialog() })

  const handleCreate = () => {
    createList.mutate(
      { name: newName, storeId: newStoreId },
      {
        onSuccess: (list) => {
          setCreateOpen(false)
          setNewName('')
          setNewStoreId(null)
          navigate({ to: '/shopping-lists/$id', params: { id: list.id } })
        },
      }
    )
  }

  const confirmDelete = () => {
    if (deleteId) {
      deleteListMutation.mutate(deleteId, {
        onSuccess: () => setDeleteId(null),
      })
    }
  }

  const activeLists = data?.items.filter((l) => l.status === 'Active') ?? []
  const completedLists = data?.items.filter((l) => l.status === 'Completed') ?? []

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Shopping Lists</h1>
          <p className="text-muted-foreground">Manage your shopping lists</p>
        </div>
        <Button onClick={openCreateDialog}>
          <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
          New List
          <Kbd>C</Kbd>
        </Button>
      </div>

      {error && (
        <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
          <p className="font-medium">Error loading shopping lists</p>
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
        <>
          {activeLists.length > 0 && (
            <div className="space-y-3">
              <h2 className="text-lg font-semibold">Active</h2>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                {activeLists.map((list) => {
                  const store = stores.find((s) => s.id === list.storeId)
                  return (
                    <Card
                      key={list.id}
                      className="cursor-pointer transition-shadow hover:shadow-md"
                      onClick={() => navigate({ to: '/shopping-lists/$id', params: { id: list.id } })}
                    >
                      <CardHeader className="flex flex-row items-center gap-3 pb-2">
                        <HugeiconsIcon icon={ShoppingCart01Icon} className="h-5 w-5 text-muted-foreground" />
                        <CardTitle className="text-lg">{list.name}</CardTitle>
                      </CardHeader>
                      <CardContent>
                        {store && (
                          <p className="text-sm text-muted-foreground">{store.name}</p>
                        )}
                        <div className="mt-2 flex items-center gap-2">
                          <span className="text-sm text-muted-foreground">
                            {list.checkedCount}/{list.itemCount} items
                          </span>
                          {list.itemCount > 0 && (
                            <div className="flex-1 rounded-full bg-muted h-2">
                              <div
                                className="rounded-full bg-primary h-2 transition-all"
                                style={{ width: `${(list.checkedCount / list.itemCount) * 100}%` }}
                              />
                            </div>
                          )}
                        </div>
                      </CardContent>
                    </Card>
                  )
                })}
              </div>
            </div>
          )}

          {completedLists.length > 0 && (
            <div className="space-y-3">
              <h2 className="text-lg font-semibold text-muted-foreground">Completed</h2>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                {completedLists.map((list) => {
                  const store = stores.find((s) => s.id === list.storeId)
                  return (
                    <Card
                      key={list.id}
                      className="cursor-pointer opacity-60 transition-shadow hover:shadow-md"
                      onClick={() => navigate({ to: '/shopping-lists/$id', params: { id: list.id } })}
                    >
                      <CardHeader className="flex flex-row items-center gap-3 pb-2">
                        <HugeiconsIcon icon={ShoppingCart01Icon} className="h-5 w-5 text-muted-foreground" />
                        <CardTitle className="text-lg">{list.name}</CardTitle>
                        <Badge variant="secondary">Completed</Badge>
                      </CardHeader>
                      <CardContent>
                        {store && (
                          <p className="text-sm text-muted-foreground">{store.name}</p>
                        )}
                        <p className="text-sm text-muted-foreground mt-1">
                          {list.itemCount} item{list.itemCount !== 1 ? 's' : ''}
                        </p>
                      </CardContent>
                    </Card>
                  )
                })}
              </div>
            </div>
          )}

          {data.items.length === 0 && (
            <div className="flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
              <p className="text-muted-foreground">No shopping lists yet</p>
              <Button className="mt-4" onClick={openCreateDialog}>
                <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
                Create your first list
              </Button>
            </div>
          )}
        </>
      )}

      <Dialog open={createOpen} onOpenChange={setCreateOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>New Shopping List</DialogTitle>
          </DialogHeader>
          <form onSubmit={(e) => { e.preventDefault(); handleCreate() }}>
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="list-name">Name</Label>
                <Input
                  id="list-name"
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  placeholder="e.g. Weekly Groceries"
                  autoFocus
                />
              </div>
              <div className="space-y-2">
                <Label>Store (optional)</Label>
                <Combobox
                  items={stores}
                  value={stores.find((s) => s.id === newStoreId) ?? null}
                  onValueChange={(store: StoreDto | null) => setNewStoreId(store?.id ?? null)}
                  itemToStringLabel={(store) => store?.name ?? ''}
                >
                  <ComboboxInput placeholder="Search stores..." className="w-full" />
                  <ComboboxContent emptyMessage="No stores found.">
                    {(store: StoreDto) => (
                      <ComboboxItem key={store.id} value={store}>
                        <span className="flex-1">{store.name}</span>
                        <ComboboxItemIndicator />
                      </ComboboxItem>
                    )}
                  </ComboboxContent>
                </Combobox>
              </div>
            </div>
            <DialogFooter className="mt-4">
              <Button type="button" variant="outline" onClick={() => setCreateOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={!newName.trim() || createList.isPending}>
                {createList.isPending ? 'Creating...' : 'Create'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <AlertDialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Shopping List</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete this shopping list? This action cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteListMutation.isPending ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
