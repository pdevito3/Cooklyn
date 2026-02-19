import { useState, useEffect } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'

import { useRecipe } from '@/domain/recipes/apis/get-recipe'
import type { IngredientDto } from '@/domain/recipes/types'
import { createShoppingList } from '@/domain/shopping-lists/apis/shopping-list-mutations'
import {
  useAddItemsFromRecipe,
} from '@/domain/shopping-lists/apis/shopping-list-mutations'
import { ShoppingListKeys } from '@/domain/shopping-lists/apis/shopping-list.keys'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Kbd } from '@/components/ui/kbd'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog'
import {
  ShoppingListPicker,
  type ShoppingListPickerMode,
} from '@/components/shopping-list-picker'
import { Notification } from '@/components/notifications'

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
  const addFromRecipe = useAddItemsFromRecipe()
  const queryClient = useQueryClient()

  const [listMode, setListMode] = useState<ShoppingListPickerMode>('existing')
  const [selectedListId, setSelectedListId] = useState<string | null>(null)
  const [newListName, setNewListName] = useState('')
  const [selectedIngredientIds, setSelectedIngredientIds] = useState<string[]>(
    [],
  )

  const ingredients = recipe?.ingredients ?? []
  const allSelected =
    ingredients.length > 0 &&
    selectedIngredientIds.length === ingredients.length

  // Default all ingredients to checked when recipe loads
  useEffect(() => {
    if (recipe) {
      setSelectedIngredientIds(recipe.ingredients.map((i) => i.id))
    }
  }, [recipe])

  const toggleIngredient = (ingredientId: string) => {
    setSelectedIngredientIds((prev) =>
      prev.includes(ingredientId)
        ? prev.filter((id) => id !== ingredientId)
        : [...prev, ingredientId],
    )
  }

  const toggleAll = () => {
    if (allSelected) {
      setSelectedIngredientIds([])
    } else {
      setSelectedIngredientIds(ingredients.map((i) => i.id))
    }
  }

  // Combined mutation: create list if needed, then add items
  const addToList = useMutation({
    mutationFn: async () => {
      let targetListId: string

      if (listMode === 'new') {
        const newList = await createShoppingList({
          name: newListName || `${recipe?.title ?? 'Recipe'} ingredients`,
          storeId: null,
        })
        targetListId = newList.id
      } else {
        if (!selectedListId) throw new Error('No list selected')
        targetListId = selectedListId
      }

      return addFromRecipe.mutateAsync({
        shoppingListId: targetListId,
        dto: {
          recipeId,
          ingredientIds: selectedIngredientIds,
        },
      })
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ShoppingListKeys.lists() })
      onOpenChange(false)
      Notification.success('Items added to shopping list!')
      setSelectedListId(null)
      setNewListName('')
      setSelectedIngredientIds([])
    },
  })

  const canSubmit =
    selectedIngredientIds.length > 0 &&
    !addToList.isPending &&
    (listMode === 'new' || selectedListId)

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Add to Shopping List</DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <ShoppingListPicker
            mode={listMode}
            onModeChange={setListMode}
            selectedListId={selectedListId}
            onSelectedListIdChange={setSelectedListId}
            newListName={newListName}
            onNewListNameChange={setNewListName}
            newListPlaceholder={`${recipe?.title ?? 'Recipe'} ingredients`}
          />

          {recipe && ingredients.length > 0 && (
            <div className="space-y-2 max-h-64 overflow-y-auto">
              <div className="flex items-center gap-2 pb-1 border-b">
                <Checkbox isSelected={allSelected} onChange={toggleAll}>
                  {allSelected ? 'Deselect All' : 'Select All'}
                </Checkbox>
              </div>
              {ingredients.map((ingredient: IngredientDto) => (
                <div key={ingredient.id} className="flex items-center gap-2">
                  <Checkbox
                    isSelected={selectedIngredientIds.includes(ingredient.id)}
                    onChange={() => toggleIngredient(ingredient.id)}
                  >
                    {ingredient.rawText ||
                      ingredient.name ||
                      'Unknown ingredient'}
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
            onClick={() => addToList.mutate()}
            disabled={!canSubmit}
          >
            {addToList.isPending ? 'Adding...' : 'Add Items'}
            {!addToList.isPending && <Kbd>⌘↵</Kbd>}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
