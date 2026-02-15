import { useState, useEffect } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { ArrowLeft02Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import { useShoppingList, useUpdateShoppingList } from '@/domain/shopping-lists'
import { useStores } from '@/domain/stores'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectItemText,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

export const Route = createFileRoute('/shopping-lists/$id/edit')({
  component: ShoppingListEditPage,
})

function ShoppingListEditPage() {
  const { id } = Route.useParams()
  const navigate = useNavigate()
  const { data: list, isLoading } = useShoppingList(id)
  const { data: storesData } = useStores({ pageSize: 100 })
  const updateList = useUpdateShoppingList()

  const [name, setName] = useState('')
  const [storeId, setStoreId] = useState<string | null>(null)

  const stores = storesData?.items ?? []

  useEffect(() => {
    if (list) {
      setName(list.name)
      setStoreId(list.storeId)
    }
  }, [list])

  const handleSave = () => {
    updateList.mutate(
      { id, dto: { name, storeId } },
      {
        onSuccess: () => navigate({ to: '/shopping-lists/$id', params: { id } }),
      }
    )
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (!list) {
    return <p className="text-muted-foreground">Shopping list not found.</p>
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => navigate({ to: '/shopping-lists/$id', params: { id } })}
        >
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-4 w-4" />
        </Button>
        <h1 className="text-3xl font-bold tracking-tight">Edit Shopping List</h1>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Details</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="edit-name">Name</Label>
            <Input
              id="edit-name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              autoFocus
            />
          </div>
          <div className="space-y-2">
            <Label>Store (optional)</Label>
            <Select
              value={storeId ?? ''}
              onValueChange={(value) => setStoreId(value || null)}
            >
              <SelectTrigger>
                <SelectValue placeholder="No store" />
              </SelectTrigger>
              <SelectContent>
                {stores.map((store) => (
                  <SelectItem key={store.id} value={store.id}>
                    <SelectItemText>{store.name}</SelectItemText>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div className="flex gap-2">
            <Button onClick={handleSave} disabled={!name.trim() || updateList.isPending}>
              {updateList.isPending ? 'Saving...' : 'Save'}
            </Button>
            <Button
              variant="outline"
              onClick={() => navigate({ to: '/shopping-lists/$id', params: { id } })}
            >
              Cancel
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
