import { useState, useMemo, useCallback, useRef, useEffect } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useHotkeys } from 'react-hotkeys-hook'
import {
  ArrowLeft02Icon,
  Delete02Icon,
  Add01Icon,
  Tick02Icon,
  ArrowDown01Icon,
  ArrowUp01Icon,
  RestaurantIcon,
  Layers01Icon,
  Tag01Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import {
  useShoppingList,
  useToggleShoppingListItemCheck,
  useRemoveCheckedItems,
  useDeleteShoppingList,
  useCompleteShoppingList,
  useReopenShoppingList,
  useAddMultipleShoppingListItems,
  useDeleteShoppingListItem,
  useUpdateShoppingListItem,
} from '@/domain/shopping-lists'
import type { ShoppingListItemDto } from '@/domain/shopping-lists'
import type { StoreSectionDto } from '@/domain/store-sections'
import { useStores } from '@/domain/stores'
import { useStoreSections } from '@/domain/store-sections'
import { parseText } from '@/domain/recipes/utils/ingredient-parser'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Checkbox } from '@/components/ui/checkbox'
import { Textarea } from '@/components/ui/textarea'
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
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import {
  Combobox,
  ComboboxInput,
  ComboboxContent,
  ComboboxItem,
  ComboboxItemIndicator,
} from '@/components/ui/combobox'
import { Kbd } from '@/components/ui/kbd'
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
  const addItems = useAddMultipleShoppingListItems()
  const deleteItem = useDeleteShoppingListItem()
  const updateItem = useUpdateShoppingListItem()

  const textareaRef = useRef<HTMLTextAreaElement>(null)
  const [deleteOpen, setDeleteOpen] = useState(false)
  const [quickAddText, setQuickAddText] = useState('')
  const [checkedOpen, setCheckedOpen] = useState(true)
  const [addFromRecipeOpen, setAddFromRecipeOpen] = useState(false)
  const [addFromCollectionOpen, setAddFromCollectionOpen] = useState(false)
  const [categorizeItem, setCategorizeItem] = useState<ShoppingListItemDto | null>(null)
  const [categorizeSection, setCategorizeSection] = useState<StoreSectionDto | null>(null)

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

  useEffect(() => {
    if (!isLoading && list && list.status !== 'Completed') {
      textareaRef.current?.focus()
    }
  }, [isLoading, list])

  useHotkeys('e', () => {
    if (list) navigate({ to: '/shopping-lists/$id/edit', params: { id } })
  })
  useHotkeys('delete', () => {
    if (list) setDeleteOpen(true)
  })

  const handleQuickAdd = () => {
    const text = quickAddText.trim()
    if (!text) return
    const parsed = parseText(text)
    const items = parsed.map((ingredient) => ({
      name: ingredient.name ?? ingredient.rawText,
      quantity: ingredient.amount ?? null,
      unit: ingredient.unit ?? null,
      storeSectionId: null,
      notes: null,
    }))
    if (items.length === 0) return
    addItems.mutate(
      { shoppingListId: id, items },
      { onSuccess: () => setQuickAddText('') }
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

  const openCategorize = useCallback((e: React.MouseEvent, item: ShoppingListItemDto) => {
    e.stopPropagation()
    const currentSection = item.storeSectionId
      ? sections.find((s) => s.id === item.storeSectionId) ?? null
      : null
    setCategorizeSection(currentSection)
    setCategorizeItem(item)
  }, [sections])

  const handleCategorizeSubmit = () => {
    if (!categorizeItem) return
    updateItem.mutate(
      {
        shoppingListId: id,
        itemId: categorizeItem.id,
        dto: {
          name: categorizeItem.name,
          quantity: categorizeItem.quantity,
          unit: categorizeItem.unit,
          storeSectionId: categorizeSection?.id ?? null,
          notes: categorizeItem.notes,
        },
      },
      {
        onSuccess: () => setCategorizeItem(null),
      }
    )
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
    <div className="max-w-5xl space-y-6">
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
            <Kbd>E</Kbd>
          </Button>
          <Button variant="destructive" onClick={() => setDeleteOpen(true)}>
            Delete
            <Kbd>⌫</Kbd>
          </Button>
        </div>
      </div>

      {/* Quick Add */}
      {!isCompleted && (
        <div className="space-y-2">
          <div className="flex gap-2">
            <Textarea
              ref={textareaRef}
              placeholder={"Add items, one per line...\ne.g. 1 bag flour, 2 Tbsp salt"}
              value={quickAddText}
              onChange={(e) => setQuickAddText(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter' && !e.shiftKey && !quickAddText.includes('\n')) {
                  e.preventDefault()
                  handleQuickAdd()
                }
              }}
              className="min-h-10"
            />
            <Button onClick={handleQuickAdd} disabled={!quickAddText.trim() || addItems.isPending} className="self-end">
              <HugeiconsIcon icon={Add01Icon} className="h-4 w-4" />
            </Button>
          </div>
          <div className="flex gap-2">
            <Button variant="outline" size="sm" onClick={() => setAddFromRecipeOpen(true)}>
              <HugeiconsIcon icon={RestaurantIcon} className="mr-2 h-4 w-4" />
              Recipe
            </Button>
            <Button variant="outline" size="sm" onClick={() => setAddFromCollectionOpen(true)}>
              <HugeiconsIcon icon={Layers01Icon} className="mr-2 h-4 w-4" />
              Collection
            </Button>
          </div>
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
                          {formatQuantity(item) && (
                            <span className="text-sm text-muted-foreground">
                              {formatQuantity(item)}
                            </span>
                          )}
                          <span className="font-medium">{item.name}</span>
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
                        className="md:opacity-0 md:group-hover:opacity-100 h-7 w-7"
                        onClick={(e) => openCategorize(e, item)}
                      >
                        <HugeiconsIcon icon={Tag01Icon} className="h-3.5 w-3.5" />
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
                        {formatQuantity(item) && (
                          <span className="text-sm text-muted-foreground line-through mr-2">
                            {formatQuantity(item)}
                          </span>
                        )}
                        <span className="text-muted-foreground line-through">{item.name}</span>
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

      {/* Categorize Item Dialog */}
      <Dialog open={categorizeItem !== null} onOpenChange={(open) => !open && setCategorizeItem(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Categorize "{categorizeItem?.name}"</DialogTitle>
          </DialogHeader>
          <form onSubmit={(e) => { e.preventDefault(); handleCategorizeSubmit() }}>
            <div className="space-y-2">
              <Combobox
                items={sections}
                value={categorizeSection}
                onValueChange={(section: StoreSectionDto | null) => setCategorizeSection(section)}
                itemToStringLabel={(section) => section?.name ?? ''}
              >
                <ComboboxInput placeholder="Search sections..." className="w-full" autoFocus />
                <ComboboxContent emptyMessage="No sections found.">
                  {(section: StoreSectionDto) => (
                    <ComboboxItem key={section.id} value={section}>
                      <span className="flex-1">{section.name}</span>
                      <ComboboxItemIndicator />
                    </ComboboxItem>
                  )}
                </ComboboxContent>
              </Combobox>
            </div>
            <DialogFooter className="mt-4">
              <Button type="button" variant="outline" onClick={() => setCategorizeItem(null)}>
                Cancel
              </Button>
              <Button type="submit" disabled={updateItem.isPending}>
                {updateItem.isPending ? 'Saving...' : 'Save'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

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
