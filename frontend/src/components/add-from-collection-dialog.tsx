import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'

import { apiClient } from '@/lib/api-client'
import { useAddItemsFromCollection } from '@/domain/shopping-lists'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectItemText,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

interface ItemCollectionSummary {
  id: string
  name: string
}

interface AddFromCollectionDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  shoppingListId: string
}

export function AddFromCollectionDialog({
  open,
  onOpenChange,
  shoppingListId,
}: AddFromCollectionDialogProps) {
  const addFromCollection = useAddItemsFromCollection()
  const [selectedCollectionId, setSelectedCollectionId] = useState<string | null>(null)

  // Fetch collections when dialog is open
  // Once Phase 6 is implemented, replace this with useItemCollections hook
  const { data: collections = [] } = useQuery({
    queryKey: ['item-collections', 'list-for-dialog'],
    queryFn: async () => {
      const response = await apiClient.get<ItemCollectionSummary[]>('/api/v1/itemcollections', {
        params: { pageSize: 100 },
      })
      return response.data
    },
    enabled: open,
  })

  const handleAdd = () => {
    if (!selectedCollectionId) return
    addFromCollection.mutate(
      {
        shoppingListId,
        dto: { itemCollectionId: selectedCollectionId },
      },
      {
        onSuccess: () => {
          onOpenChange(false)
          setSelectedCollectionId(null)
        },
      }
    )
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Add from Collection</DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <Select
            value={selectedCollectionId ?? ''}
            onValueChange={setSelectedCollectionId}
          >
            <SelectTrigger>
              <SelectValue placeholder="Select a collection">
                {(value: unknown) => {
                  if (value === null || value === undefined) {
                    return <span className="text-muted-foreground">Select a collection</span>
                  }
                  return collections.find((c) => c.id === value)?.name ?? String(value)
                }}
              </SelectValue>
            </SelectTrigger>
            <SelectContent>
              {collections.map((collection) => (
                <SelectItem key={collection.id} value={collection.id}>
                  <SelectItemText>{collection.name}</SelectItemText>
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button
            onClick={handleAdd}
            disabled={!selectedCollectionId || addFromCollection.isPending}
          >
            {addFromCollection.isPending ? 'Adding...' : 'Add Items'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
