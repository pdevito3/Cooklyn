import { useState, useEffect } from 'react'

import { useRecipes } from '@/domain/recipes/apis/get-recipes'
import { useRecipe } from '@/domain/recipes/apis/get-recipe'
import type { IngredientDto } from '@/domain/recipes/types'
import { useAddItemsFromRecipe } from '@/domain/shopping-lists/apis/shopping-list-mutations'
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

interface AddFromRecipeDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  shoppingListId: string
}

export function AddFromRecipeDialog({
  open,
  onOpenChange,
  shoppingListId,
}: AddFromRecipeDialogProps) {
  const { data: recipesData } = useRecipes({ pageSize: 100 })
  const addFromRecipe = useAddItemsFromRecipe()

  const [selectedRecipeId, setSelectedRecipeId] = useState<string | null>(null)
  const [selectedIngredientIds, setSelectedIngredientIds] = useState<string[]>([])

  const { data: selectedRecipe } = useRecipe(selectedRecipeId ?? '')

  const recipes = recipesData?.items ?? []
  const ingredients = selectedRecipe?.ingredients ?? []
  const allSelected = ingredients.length > 0 && selectedIngredientIds.length === ingredients.length

  // Default all ingredients to checked when recipe is selected
  useEffect(() => {
    if (selectedRecipe) {
      setSelectedIngredientIds(selectedRecipe.ingredients.map((i) => i.id))
    }
  }, [selectedRecipe])

  const handleRecipeSelect = (recipeId: string | null) => {
    setSelectedRecipeId(recipeId)
    setSelectedIngredientIds([])
  }

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
    if (!selectedRecipeId || selectedIngredientIds.length === 0) return
    addFromRecipe.mutate(
      {
        shoppingListId,
        dto: {
          recipeId: selectedRecipeId,
          ingredientIds: selectedIngredientIds,
        },
      },
      {
        onSuccess: () => {
          onOpenChange(false)
          setSelectedRecipeId(null)
          setSelectedIngredientIds([])
        },
      }
    )
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Add from Recipe</DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <div className="space-y-2">
            <Select
              value={selectedRecipeId ?? ''}
              onValueChange={handleRecipeSelect}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a recipe">
                  {(value: unknown) => {
                    if (value === null || value === undefined) {
                      return <span className="text-muted-foreground">Select a recipe</span>
                    }
                    return recipes.find((r) => r.id === value)?.title ?? String(value)
                  }}
                </SelectValue>
              </SelectTrigger>
              <SelectContent>
                {recipes.map((recipe) => (
                  <SelectItem key={recipe.id} value={recipe.id}>
                    <SelectItemText>{recipe.title}</SelectItemText>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {selectedRecipe && ingredients.length > 0 && (
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
            disabled={!selectedRecipeId || selectedIngredientIds.length === 0 || addFromRecipe.isPending}
          >
            {addFromRecipe.isPending ? 'Adding...' : 'Add Items'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
