import { useState, useMemo } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import {
  ArrowLeft02Icon,
  Delete02Icon,
  Add01Icon,
  Tick02Icon,
  ArrowDown01Icon,
  ArrowUp01Icon,
  RestaurantIcon,
  Layers01Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import {
  useShoppingList,
  useToggleShoppingListItemCheck,
  useRemoveCheckedItems,
  useDeleteShoppingList,
  useCompleteShoppingList,
  useReopenShoppingList,
  useAddShoppingListItem,
  useDeleteShoppingListItem,
} from '@/domain/shopping-lists'
import type { ShoppingListItemDto } from '@/domain/shopping-lists'
import { useStores } from '@/domain/stores'
import { useStoreSections } from '@/domain/store-sections'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Checkbox } from '@/components/ui/checkbox'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
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
  Select,
  SelectContent,
  SelectItem,
  SelectItemText,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { AddFromRecipeDialog } from '@/components/add-from-recipe-dialog'
import { AddFromCollectionDialog } from '@/components/add-from-collection-dialog'

export const Route = createFileRoute('/shopping-lists/$id/')({
  component: ShoppingListDetailPage,
})

function formatQuantity(item: ShoppingListItemDto): string {
  const parts: string[] = []
  if (item.quantity != null) parts.push(String(item.quantity))
  if (item.unit) parts.push(item.unit)
  return parts.join(' ')
}

function ShoppingListDetailPage() {
  const { id } = Route.useParams()
  const navigate = useNavigate()
  const { data: list, isLoading } = useShoppingList(id)
  const { data: storesData } = useStores({ pageSize: 100 })
  const { data: sectionsData } = useStoreSections({ pageSize: 100 })
  const toggleCheck = useToggleShoppingListItemCheck()
  const removeChecked = useRemoveCheckedItems()
  const deleteListMutation = useDeleteShoppingList()
  const completeList = useCompleteShoppingList()
  const reopenList = useReopenShoppingList()
  const addItem = useAddShoppingListItem()
  const deleteItem = useDeleteShoppingListItem()

  const [deleteOpen, setDeleteOpen] = useState(false)
  const [quickAddName, setQuickAddName] = useState('')
  const [quickAddSectionId, setQuickAddSectionId] = useState<string | null>(null)
  const [checkedOpen, setCheckedOpen] = useState(true)
  const [addFromRecipeOpen, setAddFromRecipeOpen] = useState(false)
  const [addFromCollectionOpen, setAddFromCollectionOpen] = useState(false)

  const stores = storesData?.items ?? []
  const sectionItems = sectionsData?.items
  const sections = sectionItems ?? []
  const store = list?.storeId ? stores.find((s) => s.id === list.storeId) : null

  const { uncheckedGroups, checkedItems } = useMemo(() => {
    if (!list) return { uncheckedGroups: [], checkedItems: [] }

    const unchecked = list.items.filter((i) => !i.isChecked)
    const checked = list.items
      .filter((i) => i.isChecked)
      .sort((a, b) => {
        if (a.checkedOn && b.checkedOn) return b.checkedOn.localeCompare(a.checkedOn)
        return 0
      })

    // Group unchecked items by store section
    const groups = new Map<string | null, ShoppingListItemDto[]>()
    for (const item of unchecked) {
      const key = item.storeSectionId
      if (!groups.has(key)) groups.set(key, [])
      groups.get(key)!.push(item)
    }

    // Sort groups by store aisle order if we have a store
    const storeAisles = store?.storeAisles ?? []
    const groupEntries = Array.from(groups.entries()).sort(([a], [b]) => {
      if (a === null && b === null) return 0
      if (a === null) return 1
      if (b === null) return -1
      const aOrder = storeAisles.find((aisle) => aisle.storeSectionId === a)?.sortOrder ?? 999
      const bOrder = storeAisles.find((aisle) => aisle.storeSectionId === b)?.sortOrder ?? 999
      return aOrder - bOrder
    })

    // Sort items within each group by sortOrder
    for (const [, items] of groupEntries) {
      items.sort((a, b) => a.sortOrder - b.sortOrder)
    }

    return {
      uncheckedGroups: groupEntries.map(([sectionId, items]) => ({
        sectionId,
        sectionName: sectionId ? (
          // Use custom aisle name if available, otherwise section name
          storeAisles.find((a) => a.storeSectionId === sectionId)?.customName ??
          sections.find((s) => s.id === sectionId)?.name ??
          'Unknown'
        ) : 'Uncategorized',
        items,
      })),
      checkedItems: checked,
    }
  }, [list, store, sectionItems, sections])

  const handleQuickAdd = () => {
    const name = quickAddName.trim()
    if (!name) return
    addItem.mutate(
      {
        shoppingListId: id,
        dto: { name, quantity: null, unit: null, storeSectionId: quickAddSectionId, notes: null },
      },
      { onSuccess: () => setQuickAddName('') }
    )
  }

  const handleToggleCheck = (itemId: string) => {
    toggleCheck.mutate({ shoppingListId: id, itemId })
  }

  const handleRemoveChecked = () => {
    removeChecked.mutate(id)
  }

  const handleDelete = () => {
    deleteListMutation.mutate(id, {
      onSuccess: () => navigate({ to: '/shopping-lists' }),
    })
  }

  const handleDeleteItem = (itemId: string) => {
    deleteItem.mutate({ shoppingListId: id, itemId })
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
      </div>
    )
  }

  if (!list) {
    return <p className="text-muted-foreground">Shopping list not found.</p>
  }

  const isCompleted = list.status === 'Completed'

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={() => navigate({ to: '/shopping-lists' })}>
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-4 w-4" />
        </Button>
        <div className="flex-1">
          <div className="flex items-center gap-3">
            <h1 className="text-3xl font-bold tracking-tight">{list.name}</h1>
            {isCompleted && <Badge variant="secondary">Completed</Badge>}
          </div>
          {store && <p className="text-muted-foreground">{store.name}</p>}
        </div>
        <div className="flex gap-2">
          {isCompleted ? (
            <Button variant="outline" onClick={() => reopenList.mutate(id)} disabled={reopenList.isPending}>
              Reopen
            </Button>
          ) : (
            <Button variant="outline" onClick={() => completeList.mutate(id)} disabled={completeList.isPending}>
              <HugeiconsIcon icon={Tick02Icon} className="mr-2 h-4 w-4" />
              Complete
            </Button>
          )}
          <Button variant="outline" onClick={() => navigate({ to: '/shopping-lists/$id/edit', params: { id } })}>
            Edit
          </Button>
          <Button variant="destructive" onClick={() => setDeleteOpen(true)}>
            Delete
          </Button>
        </div>
      </div>

      {/* Quick Add */}
      {!isCompleted && (
        <div className="flex gap-2">
          <Input
            placeholder="Add an item..."
            value={quickAddName}
            onChange={(e) => setQuickAddName(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleQuickAdd()}
          />
          {store && store.storeAisles.length > 0 && (
            <Select
              value={quickAddSectionId ?? ''}
              onValueChange={(value) => setQuickAddSectionId(value || null)}
            >
              <SelectTrigger className="w-40">
                <SelectValue placeholder="Section" />
              </SelectTrigger>
              <SelectContent>
                {[...store.storeAisles]
                  .sort((a, b) => a.sortOrder - b.sortOrder)
                  .map((aisle) => {
                    const section = sections.find((s) => s.id === aisle.storeSectionId)
                    return (
                      <SelectItem key={aisle.id} value={aisle.storeSectionId}>
                        <SelectItemText>{aisle.customName ?? section?.name ?? 'Unknown'}</SelectItemText>
                      </SelectItem>
                    )
                  })}
              </SelectContent>
            </Select>
          )}
          <Button onClick={handleQuickAdd} disabled={!quickAddName.trim() || addItem.isPending}>
            <HugeiconsIcon icon={Add01Icon} className="h-4 w-4" />
          </Button>
          <Button variant="outline" onClick={() => setAddFromRecipeOpen(true)}>
            <HugeiconsIcon icon={RestaurantIcon} className="mr-2 h-4 w-4" />
            Recipe
          </Button>
          <Button variant="outline" onClick={() => setAddFromCollectionOpen(true)}>
            <HugeiconsIcon icon={Layers01Icon} className="mr-2 h-4 w-4" />
            Collection
          </Button>
        </div>
      )}

      {/* Unchecked Items */}
      {uncheckedGroups.length > 0 ? (
        <div className="space-y-4">
          {uncheckedGroups.map((group) => (
            <Card key={group.sectionId ?? 'uncategorized'}>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground uppercase tracking-wide">
                  {group.sectionName}
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-1">
                {group.items.map((item) => (
                  <div
                    key={item.id}
                    className="flex items-center gap-3 rounded-md p-2 hover:bg-muted/50 group"
                  >
                    <Checkbox
                      isSelected={false}
                      onChange={() => handleToggleCheck(item.id)}
                      className="flex-1 min-w-0"
                    >
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2">
                          <span className="font-medium">{item.name}</span>
                          {formatQuantity(item) && (
                            <span className="text-sm text-muted-foreground">
                              {formatQuantity(item)}
                            </span>
                          )}
                        </div>
                        {item.notes && (
                          <p className="text-xs text-muted-foreground">{item.notes}</p>
                        )}
                      </div>
                    </Checkbox>
                    {!isCompleted && (
                      <Button
                        variant="ghost"
                        size="icon"
                        className="opacity-0 group-hover:opacity-100 h-7 w-7"
                        onClick={() => handleDeleteItem(item.id)}
                      >
                        <HugeiconsIcon icon={Delete02Icon} className="h-3.5 w-3.5" />
                      </Button>
                    )}
                  </div>
                ))}
              </CardContent>
            </Card>
          ))}
        </div>
      ) : (
        !isCompleted && checkedItems.length === 0 && (
          <div className="flex flex-col items-center justify-center rounded-lg border border-dashed py-12">
            <p className="text-muted-foreground">No items yet. Add some above!</p>
          </div>
        )
      )}

      {/* Checked Items */}
      {checkedItems.length > 0 && (
        <Collapsible open={checkedOpen} onOpenChange={setCheckedOpen}>
          <Card>
            <CardHeader className="pb-2">
              <div className="flex items-center justify-between">
                <CollapsibleTrigger className="flex items-center gap-2 hover:opacity-80">
                  <HugeiconsIcon
                    icon={checkedOpen ? ArrowDown01Icon : ArrowUp01Icon}
                    className="h-4 w-4"
                  />
                  <CardTitle className="text-sm font-medium text-muted-foreground">
                    Checked ({checkedItems.length})
                  </CardTitle>
                </CollapsibleTrigger>
                {!isCompleted && (
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={handleRemoveChecked}
                    disabled={removeChecked.isPending}
                  >
                    Remove all checked
                  </Button>
                )}
              </div>
            </CardHeader>
            <CollapsibleContent>
              <CardContent className="space-y-1">
                {checkedItems.map((item) => (
                  <div
                    key={item.id}
                    className="flex items-center gap-3 rounded-md p-2 hover:bg-muted/50 group"
                  >
                    <Checkbox
                      isSelected={true}
                      onChange={() => handleToggleCheck(item.id)}
                      className="flex-1 min-w-0"
                    >
                      <div className="flex-1 min-w-0">
                        <span className="text-muted-foreground line-through">{item.name}</span>
                        {formatQuantity(item) && (
                          <span className="text-sm text-muted-foreground ml-2">
                            {formatQuantity(item)}
                          </span>
                        )}
                      </div>
                    </Checkbox>
                    {!isCompleted && (
                      <Button
                        variant="ghost"
                        size="icon"
                        className="opacity-0 group-hover:opacity-100 h-7 w-7"
                        onClick={() => handleDeleteItem(item.id)}
                      >
                        <HugeiconsIcon icon={Delete02Icon} className="h-3.5 w-3.5" />
                      </Button>
                    )}
                  </div>
                ))}
              </CardContent>
            </CollapsibleContent>
          </Card>
        </Collapsible>
      )}

      {/* Delete Dialog */}
      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Shopping List</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete "{list.name}"? This action cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteListMutation.isPending ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Add from Recipe Dialog */}
      <AddFromRecipeDialog
        open={addFromRecipeOpen}
        onOpenChange={setAddFromRecipeOpen}
        shoppingListId={id}
      />

      {/* Add from Collection Dialog */}
      <AddFromCollectionDialog
        open={addFromCollectionOpen}
        onOpenChange={setAddFromCollectionOpen}
        shoppingListId={id}
      />
    </div>
  )
}
