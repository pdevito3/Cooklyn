import { useState, useEffect, useRef } from 'react'
import { format } from 'date-fns'

import { useShoppingLists } from '@/domain/shopping-lists/apis/get-shopping-lists'
import {
  useCreateShoppingList,
  useAddMultipleShoppingListItems,
} from '@/domain/shopping-lists/apis/shopping-list-mutations'
import type { ShoppingListItemForCreationDto } from '@/domain/shopping-lists/types'
import { useDefaultStore } from '@/domain/settings/apis/get-setting'
import { parseText } from '@/domain/recipes/utils/ingredient-parser'
import { Button } from '@/components/ui/button'
import { Kbd } from '@/components/ui/kbd'
import { Textarea } from '@/components/ui/textarea'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
  SelectItemText,
} from '@/components/ui/select'
import {
  ResponsiveDialog,
  ResponsiveDialogContent,
  ResponsiveDialogHeader,
  ResponsiveDialogTitle,
  ResponsiveDialogFooter,
} from '@/components/ui/responsive-dialog'

interface QuickAddDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

const NEW_LIST_VALUE = '__new__'

export function QuickAddDialog({ open, onOpenChange }: QuickAddDialogProps) {
  const { data: listsData } = useShoppingLists({ pageSize: 100 })
  const { data: defaultStoreId } = useDefaultStore()
  const createList = useCreateShoppingList()
  const addItems = useAddMultipleShoppingListItems()

  const [selectedListId, setSelectedListId] = useState<string | null>(null)
  const [isCreatingNewList, setIsCreatingNewList] = useState(false)
  const [newListName, setNewListName] = useState('')
  const [text, setText] = useState('')
  const textareaRef = useRef<HTMLTextAreaElement>(null)

  const activeLists = (
    listsData?.items.filter((l) => l.status === 'Active') ?? []
  ).toSorted(
    (a, b) => new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime(),
  )

  // Auto-select first active list when dialog opens
  const firstActiveListId = activeLists[0]?.id ?? null
  useEffect(() => {
    if (open && firstActiveListId && !selectedListId) {
      setSelectedListId(firstActiveListId)
    }
  }, [open, firstActiveListId, selectedListId])

  // Focus textarea after dialog opens
  useEffect(() => {
    if (open) {
      const timer = setTimeout(() => textareaRef.current?.focus(), 100)
      return () => clearTimeout(timer)
    }
  }, [open])

  // Reset state when dialog closes
  useEffect(() => {
    if (!open) {
      setIsCreatingNewList(false)
      setNewListName('')
      setSelectedListId(null)
    }
  }, [open])

  const handleSelectChange = (value: string) => {
    if (value === NEW_LIST_VALUE) {
      setIsCreatingNewList(true)
      setNewListName(`Groceries - ${format(new Date(), 'MM/dd/yyyy')}`)
      setSelectedListId(null)
    } else {
      setIsCreatingNewList(false)
      setNewListName('')
      setSelectedListId(value)
    }
  }

  const handleSubmit = async () => {
    if (!text.trim()) return

    const parsed = parseText(text)
    const items: ShoppingListItemForCreationDto[] = parsed.map(
      (ingredient) => ({
        name: ingredient.name ?? ingredient.rawText,
        quantity: ingredient.amount ?? null,
        unit: ingredient.unit ?? null,
        storeSectionId: null,
        notes: null,
      }),
    )

    if (items.length === 0) return

    let listId = selectedListId

    // Create new list if needed
    if (isCreatingNewList && newListName.trim()) {
      try {
        const newList = await createList.mutateAsync({
          name: newListName,
          storeId: defaultStoreId ?? null,
        })
        listId = newList.id
      } catch {
        return
      }
    }

    if (!listId) return

    addItems.mutate(
      { shoppingListId: listId, items },
      {
        onSuccess: () => {
          setText('')
          onOpenChange(false)
        },
      },
    )
  }

  const isSubmitting = addItems.isPending || createList.isPending
  const canSubmit =
    text.trim() &&
    (selectedListId || (isCreatingNewList && newListName.trim())) &&
    !isSubmitting

  return (
    <ResponsiveDialog open={open} onOpenChange={onOpenChange}>
      <ResponsiveDialogContent>
        <ResponsiveDialogHeader>
          <ResponsiveDialogTitle>Quick Add Items</ResponsiveDialogTitle>
        </ResponsiveDialogHeader>
        <div className="space-y-4 mt-4">
          <div className="space-y-2">
            <Label>Shopping List</Label>
            <Select
              value={
                isCreatingNewList ? NEW_LIST_VALUE : (selectedListId ?? '')
              }
              onValueChange={handleSelectChange}
            >
              <SelectTrigger>
                <SelectValue>
                  {(value: string) => {
                    if (!value)
                      return (
                        <span className="text-muted-foreground">
                          Select a list
                        </span>
                      )
                    if (value === NEW_LIST_VALUE) return '+ Create new list'
                    const list = activeLists.find((l) => l.id === value)
                    return list?.name ?? value
                  }}
                </SelectValue>
              </SelectTrigger>
              <SelectContent>
                {activeLists.map((list) => (
                  <SelectItem key={list.id} value={list.id}>
                    <SelectItemText>{list.name}</SelectItemText>
                  </SelectItem>
                ))}
                <SelectItem value={NEW_LIST_VALUE}>
                  <SelectItemText>+ Create new list</SelectItemText>
                </SelectItem>
              </SelectContent>
            </Select>
            {isCreatingNewList && (
              <Input
                placeholder="New list name"
                value={newListName}
                onChange={(e) => setNewListName(e.target.value)}
                autoFocus
              />
            )}
          </div>
          <div className="space-y-2">
            <Label>Items (one per line)</Label>
            <Textarea
              ref={textareaRef}
              placeholder={'2 cups flour\n1 lb chicken breast\nmilk\n3 eggs'}
              value={text}
              onChange={(e) => setText(e.target.value)}
              onKeyDown={(e) => {
                if (
                  e.key === 'Enter' &&
                  (e.metaKey || e.ctrlKey) &&
                  canSubmit
                ) {
                  e.preventDefault()
                  handleSubmit()
                }
              }}
              rows={6}
            />
          </div>
        </div>
        <ResponsiveDialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={!canSubmit}>
            {isSubmitting ? 'Adding...' : 'Add Items'}
            {!isSubmitting && <Kbd>⌘↵</Kbd>}
          </Button>
        </ResponsiveDialogFooter>
      </ResponsiveDialogContent>
    </ResponsiveDialog>
  )
}
