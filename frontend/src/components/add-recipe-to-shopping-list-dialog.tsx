import { useState, useEffect } from 'react'

import { useRecipe } from '@/domain/recipes'
import type { IngredientDto } from '@/domain/recipes'
import { useShoppingLists, useAddItemsFromRecipe } from '@/domain/shopping-lists'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
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

interface AddRecipeToShoppingListDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  recipeId: string
}

export function AddRecipeToShoppingListDialog({
  open,
  onOpenChange,
  recipeId,
}: AddRecipeToShoppingListDialogProps) {
  const { data: recipe } = useRecipe(recipeId)
  const { data: listsData } = useShoppingLists({ pageSize: 100 })
  const addFromRecipe = useAddItemsFromRecipe()

  const [selectedListId, setSelectedListId] = useState<string | null>(null)
  const [selectedIngredientIds, setSelectedIngredientIds] = useState<string[]>([])

  const activeLists = listsData?.items.filter((l) => l.status === 'Active') ?? []
  const ingredients = recipe?.ingredients ?? []
  const allSelected = ingredients.length > 0 && selectedIngredientIds.length === ingredients.length

  // Default all ingredients to checked when recipe loads
  useEffect(() => {
    if (recipe) {
      setSelectedIngredientIds(recipe.ingredients.map((i) => i.id))
    }
  }, [recipe])

  // Auto-select first active list
  const firstActiveListId = activeLists[0]?.id ?? null
  useEffect(() => {
    if (firstActiveListId && !selectedListId) {
      setSelectedListId(firstActiveListId)
    }
  }, [firstActiveListId, selectedListId])

  const toggleIngredient = (ingredientId: string) => {
    setSelectedIngredientIds((prev) =>
      prev.includes(ingredientId)
        ? prev.filter((id) => id !== ingredientId)
        : [...prev, ingredientId]
    )
  }

  const toggleAll = () => {
    if (allSelected) {
      setSelectedIngredientIds([])
    } else {
      setSelectedIngredientIds(ingredients.map((i) => i.id))
    }
  }

  const handleAdd = () => {
    if (!selectedListId || selectedIngredientIds.length === 0) return
    addFromRecipe.mutate(
      {
        shoppingListId: selectedListId,
        dto: {
          recipeId,
          ingredientIds: selectedIngredientIds,
        },
      },
      {
        onSuccess: () => {
          onOpenChange(false)
          setSelectedListId(null)
          setSelectedIngredientIds([])
        },
      }
    )
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Add to Shopping List</DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <div className="space-y-2">
            <Select
              value={selectedListId ?? ''}
              onValueChange={setSelectedListId}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a shopping list">
                  {(value: unknown) => {
                    if (value === null || value === undefined) {
                      return <span className="text-muted-foreground">Select a shopping list</span>
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
              </SelectContent>
            </Select>
          </div>

          {recipe && ingredients.length > 0 && (
            <div className="space-y-2 max-h-64 overflow-y-auto">
              <div className="flex items-center gap-2 pb-1 border-b">
                <Checkbox
                  isSelected={allSelected}
                  onChange={toggleAll}
                >
                  {allSelected ? 'Deselect All' : 'Select All'}
                </Checkbox>
              </div>
              {ingredients.map((ingredient: IngredientDto) => (
                <div key={ingredient.id} className="flex items-center gap-2">
                  <Checkbox
                    isSelected={selectedIngredientIds.includes(ingredient.id)}
                    onChange={() => toggleIngredient(ingredient.id)}
                  >
                    {ingredient.rawText || ingredient.name || 'Unknown ingredient'}
                  </Checkbox>
                </div>
              ))}
            </div>
          )}
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button
            onClick={handleAdd}
            disabled={!selectedListId || selectedIngredientIds.length === 0 || addFromRecipe.isPending}
          >
            {addFromRecipe.isPending ? 'Adding...' : 'Add Items'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
