import { useState, useCallback } from 'react'
import { ArrowUp02Icon, ArrowDown02Icon, Delete02Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { IngredientForCreationDto } from '@/domain/recipes'
import { parseText } from '@/domain/recipes/utils/ingredient-parser'

interface IngredientEditorProps {
  value: IngredientForCreationDto[]
  onChange: (ingredients: IngredientForCreationDto[]) => void
}

export function IngredientEditor({ value, onChange }: IngredientEditorProps) {
  const [mode, setMode] = useState<'text' | 'structured'>('text')
  const [rawText, setRawText] = useState(() =>
    value.length > 0 ? value.map((i) => i.rawText).join('\n') : ''
  )

  const handleTextChange = useCallback(
    (text: string) => {
      setRawText(text)
      const parsed = parseText(text)
      onChange(parsed)
    },
    [onChange]
  )

  const handleRemoveIngredient = useCallback(
    (index: number) => {
      const updated = value.filter((_, i) => i !== index)
      // Re-assign sort orders
      const reordered = updated.map((item, i) => ({ ...item, sortOrder: i }))
      onChange(reordered)
      setRawText(reordered.map((i) => i.rawText).join('\n'))
    },
    [value, onChange]
  )

  const handleMoveIngredient = useCallback(
    (index: number, direction: 'up' | 'down') => {
      const newIndex = direction === 'up' ? index - 1 : index + 1
      if (newIndex < 0 || newIndex >= value.length) return

      const updated = [...value]
      const [moved] = updated.splice(index, 1)
      updated.splice(newIndex, 0, moved)
      const reordered = updated.map((item, i) => ({ ...item, sortOrder: i }))
      onChange(reordered)
      setRawText(reordered.map((i) => i.rawText).join('\n'))
    },
    [value, onChange]
  )

  const handleStructuredFieldChange = useCallback(
    (index: number, field: keyof IngredientForCreationDto, fieldValue: string) => {
      const updated = [...value]
      const item = { ...updated[index] }

      if (field === 'name') {
        item.name = fieldValue || null
      } else if (field === 'amountText') {
        item.amountText = fieldValue || null
        const num = Number(fieldValue)
        item.amount = isNaN(num) ? null : num
      } else if (field === 'unit') {
        item.unit = fieldValue || null
      } else if (field === 'groupName') {
        item.groupName = fieldValue || null
      }

      // Rebuild rawText
      const parts: string[] = []
      if (item.amountText) parts.push(item.amountText)
      if (item.unit) parts.push(item.unit)
      if (item.name) parts.push(item.name)
      item.rawText = parts.join(' ')

      updated[index] = item
      onChange(updated)
      setRawText(updated.map((i) => i.rawText).join('\n'))
    },
    [value, onChange]
  )

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between">
        <Label>Ingredients</Label>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => setMode(mode === 'text' ? 'structured' : 'text')}
        >
          {mode === 'text' ? 'Edit as list' : 'Edit as text'}
        </Button>
      </div>

      {mode === 'text' ? (
        <div className="space-y-3">
          <Textarea
            placeholder={`Enter ingredients, one per line...\n\nExamples:\n2 cups flour\n1 1/2 tsp salt\n3 large eggs\n1 lb chicken breast`}
            className="min-h-[200px] font-mono text-sm"
            value={rawText}
            onChange={(e) => handleTextChange(e.target.value)}
          />
          {value.length > 0 && (
            <div className="space-y-1">
              <p className="text-xs font-medium text-muted-foreground">
                Parsed preview ({value.length} ingredient{value.length !== 1 ? 's' : ''})
              </p>
              <div className="rounded-md border bg-muted/30 p-2">
                {value.map((ingredient, i) => (
                  <div
                    key={i}
                    className="flex items-baseline gap-2 py-0.5 text-sm"
                  >
                    <span className="min-w-5 text-right text-xs text-muted-foreground">
                      {i + 1}.
                    </span>
                    {ingredient.amountText && (
                      <span className="font-medium">{ingredient.amountText}</span>
                    )}
                    {ingredient.unit && (
                      <span className="text-muted-foreground">{ingredient.unit}</span>
                    )}
                    {ingredient.name && <span>{ingredient.name}</span>}
                    {!ingredient.amountText && !ingredient.unit && !ingredient.name && (
                      <span className="italic text-muted-foreground">
                        {ingredient.rawText}
                      </span>
                    )}
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      ) : (
        <div className="space-y-2">
          {value.length === 0 ? (
            <p className="py-4 text-center text-sm text-muted-foreground">
              No ingredients added yet. Switch to text mode to add ingredients.
            </p>
          ) : (
            value.map((ingredient, index) => (
              <div
                key={index}
                className="flex items-start gap-2 rounded-md border p-2"
              >
                <div className="flex flex-col gap-1 pt-1">
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="h-6 w-6"
                    disabled={index === 0}
                    onClick={() => handleMoveIngredient(index, 'up')}
                  >
                    <HugeiconsIcon icon={ArrowUp02Icon} className="h-3 w-3" />
                  </Button>
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="h-6 w-6"
                    disabled={index === value.length - 1}
                    onClick={() => handleMoveIngredient(index, 'down')}
                  >
                    <HugeiconsIcon icon={ArrowDown02Icon} className="h-3 w-3" />
                  </Button>
                </div>
                <div className="grid flex-1 gap-2 sm:grid-cols-4">
                  <Input
                    placeholder="Amount"
                    value={ingredient.amountText ?? ''}
                    onChange={(e) =>
                      handleStructuredFieldChange(index, 'amountText', e.target.value)
                    }
                  />
                  <Input
                    placeholder="Unit"
                    value={ingredient.unit ?? ''}
                    onChange={(e) =>
                      handleStructuredFieldChange(index, 'unit', e.target.value)
                    }
                  />
                  <Input
                    placeholder="Ingredient name"
                    className="sm:col-span-2"
                    value={ingredient.name ?? ''}
                    onChange={(e) =>
                      handleStructuredFieldChange(index, 'name', e.target.value)
                    }
                  />
                </div>
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8 text-destructive"
                  onClick={() => handleRemoveIngredient(index)}
                >
                  <HugeiconsIcon icon={Delete02Icon} className="h-4 w-4" />
                </Button>
              </div>
            ))
          )}
        </div>
      )}
    </div>
  )
}
