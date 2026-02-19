import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectItemText,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { useShoppingLists } from '@/domain/shopping-lists/apis/get-shopping-lists'

export type ShoppingListPickerMode = 'existing' | 'new'

interface ShoppingListPickerProps {
  selectedListId: string | null
  newListName: string
  onSelectedListIdChange: (id: string | null) => void
  onNewListNameChange: (name: string) => void
  mode: ShoppingListPickerMode
  onModeChange: (mode: ShoppingListPickerMode) => void
  newListPlaceholder?: string
}

export function ShoppingListPicker({
  selectedListId,
  newListName,
  onSelectedListIdChange,
  onNewListNameChange,
  mode,
  onModeChange,
  newListPlaceholder = 'Shopping list name',
}: ShoppingListPickerProps) {
  const { data: listsData } = useShoppingLists({ pageSize: 100 })
  const activeLists =
    listsData?.items.filter((l) => l.status === 'Active') ?? []

  return (
    <div className="space-y-2">
      <Label>Shopping List</Label>
      <div className="flex rounded-lg border bg-muted p-0.5 w-fit">
        <Button
          type="button"
          variant={mode === 'existing' ? 'default' : 'ghost'}
          size="sm"
          className="h-7 px-3 text-xs"
          onClick={() => onModeChange('existing')}
        >
          Existing list
        </Button>
        <Button
          type="button"
          variant={mode === 'new' ? 'default' : 'ghost'}
          size="sm"
          className="h-7 px-3 text-xs"
          onClick={() => onModeChange('new')}
        >
          New list
        </Button>
      </div>

      {mode === 'existing' ? (
        <Select
          value={selectedListId ?? ''}
          onValueChange={onSelectedListIdChange}
        >
          <SelectTrigger>
            <SelectValue placeholder="Select a shopping list">
              {(value: unknown) => {
                if (value === null || value === undefined || value === '') {
                  return (
                    <span className="text-muted-foreground">
                      Select a shopping list
                    </span>
                  )
                }
                const list = activeLists.find((l) => l.id === value)
                return list?.name ?? String(value)
              }}
            </SelectValue>
          </SelectTrigger>
          <SelectContent>
            {activeLists.map((list) => (
              <SelectItem key={list.id} value={list.id}>
                <SelectItemText>{list.name}</SelectItemText>
              </SelectItem>
            ))}
            {activeLists.length === 0 && (
              <div className="px-2 py-1.5 text-sm text-muted-foreground">
                No active lists
              </div>
            )}
          </SelectContent>
        </Select>
      ) : (
        <Input
          placeholder={newListPlaceholder}
          value={newListName}
          onChange={(e) => onNewListNameChange(e.target.value)}
          autoFocus
        />
      )}
    </div>
  )
}
