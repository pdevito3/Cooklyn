import { useState, useCallback } from 'react'
import { ArrowUp02Icon, ArrowDown02Icon, Delete02Icon, InformationCircleIcon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { Dialog as DialogPrimitive } from '@base-ui/react/dialog'

import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { formatUnit, type IngredientForCreationDto } from '@/domain/recipes/types'
import { parseText, ingredientsToText } from '@/domain/recipes/utils/ingredient-parser'
import { cn } from '@/lib/utils'

interface IngredientEditorProps {
  value: IngredientForCreationDto[]
  onChange: (ingredients: IngredientForCreationDto[]) => void
}

export function IngredientEditor({ value, onChange }: IngredientEditorProps) {
  const [mode, setMode] = useState<'text' | 'structured'>('text')
  const [textTab, setTextTab] = useState<'write' | 'preview'>('write')
  const [rawText, setRawText] = useState(() =>
    value.length > 0 ? ingredientsToText(value) : ''
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
      setRawText(ingredientsToText(reordered))
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
      setRawText(ingredientsToText(reordered))
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
      setRawText(ingredientsToText(updated))
    },
    [value, onChange]
  )

  const [helpOpen, setHelpOpen] = useState(false)

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Label>Ingredients</Label>
          <DialogPrimitive.Root open={helpOpen} onOpenChange={setHelpOpen}>
            <DialogPrimitive.Trigger
              className="inline-flex items-center gap-1 text-xs text-muted-foreground hover:text-foreground transition-colors"
            >
              <HugeiconsIcon icon={InformationCircleIcon} className="h-3.5 w-3.5" />
              Formatting help
            </DialogPrimitive.Trigger>
            <DialogPrimitive.Portal>
              <DialogPrimitive.Backdrop className="fixed inset-0 z-50 bg-black/50 data-[open]:animate-in data-[closed]:animate-out data-[closed]:fade-out-0 data-[open]:fade-in-0 duration-200" />
              <DialogPrimitive.Popup className="bg-background fixed left-1/2 top-1/2 z-50 w-[calc(100%-2rem)] max-w-lg -translate-x-1/2 -translate-y-1/2 rounded-lg border p-6 shadow-lg data-[open]:animate-in data-[closed]:animate-out data-[closed]:fade-out-0 data-[open]:fade-in-0 data-[closed]:zoom-out-95 data-[open]:zoom-in-95 duration-200 max-h-[85vh] overflow-y-auto">
                <DialogPrimitive.Title className="text-lg font-semibold">
                  Ingredient Formatting Guide
                </DialogPrimitive.Title>
                <DialogPrimitive.Description className="mt-1 text-sm text-muted-foreground">
                  Tips for entering ingredients in text mode.
                </DialogPrimitive.Description>
                <div className="mt-4 space-y-4 text-sm">
                  <div>
                    <h4 className="font-medium">Basic format</h4>
                    <p className="mt-1 text-muted-foreground">
                      Enter one ingredient per line as: amount unit name
                    </p>
                    <pre className="mt-1.5 rounded-md bg-muted/50 p-2 font-mono text-xs">
{`2 cups flour
1 tsp salt
3 large eggs`}
                    </pre>
                  </div>
                  <div>
                    <h4 className="font-medium">Fractions</h4>
                    <p className="mt-1 text-muted-foreground">
                      Use slashes for fractions, including mixed numbers.
                    </p>
                    <pre className="mt-1.5 rounded-md bg-muted/50 p-2 font-mono text-xs">
{`1/2 cup sugar
1 1/2 tsp baking powder
3/4 lb ground beef`}
                    </pre>
                  </div>
                  <div>
                    <h4 className="font-medium">Groups</h4>
                    <p className="mt-1 text-muted-foreground">
                      End a line with a colon to create a group header. All ingredients after it belong to that group.
                    </p>
                    <pre className="mt-1.5 rounded-md bg-muted/50 p-2 font-mono text-xs">
{`Biscuit:
2 cups flour
1 T butter

Gravy:
2 T butter
3 cups milk`}
                    </pre>
                  </div>
                  <div>
                    <h4 className="font-medium">Unit shortcuts</h4>
                    <p className="mt-1 text-muted-foreground">
                      Common abbreviations are recognized automatically.
                    </p>
                    <div className="mt-1.5 grid grid-cols-2 gap-x-4 gap-y-0.5 text-xs text-muted-foreground">
                      <span><code className="font-mono">tsp</code> = Teaspoon</span>
                      <span><code className="font-mono">tbsp</code> = Tablespoon</span>
                      <span><code className="font-mono">c</code> = Cup</span>
                      <span><code className="font-mono">oz</code> = Ounce</span>
                      <span><code className="font-mono">lb</code> = Pound</span>
                      <span><code className="font-mono">g</code> = Gram</span>
                      <span><code className="font-mono">kg</code> = Kilogram</span>
                      <span><code className="font-mono">ml</code> = Milliliter</span>
                      <span><code className="font-mono">T</code> = Tablespoon</span>
                      <span><code className="font-mono">t</code> = Teaspoon</span>
                    </div>
                  </div>
                </div>
                <div className="mt-5 flex justify-end">
                  <DialogPrimitive.Close
                    render={<Button variant="outline" size="sm">Close</Button>}
                  />
                </div>
              </DialogPrimitive.Popup>
            </DialogPrimitive.Portal>
          </DialogPrimitive.Root>
        </div>
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
        <div>
          <div className="flex border-b">
            <button
              type="button"
              className={cn(
                'px-3 py-1.5 text-sm font-medium transition-colors',
                textTab === 'write'
                  ? 'border-b-2 border-primary text-foreground'
                  : 'text-muted-foreground hover:text-foreground'
              )}
              onClick={() => setTextTab('write')}
            >
              Write
            </button>
            <button
              type="button"
              className={cn(
                'px-3 py-1.5 text-sm font-medium transition-colors',
                textTab === 'preview'
                  ? 'border-b-2 border-primary text-foreground'
                  : 'text-muted-foreground hover:text-foreground'
              )}
              onClick={() => setTextTab('preview')}
            >
              Preview
              {value.length > 0 && (
                <span className="ml-1.5 text-xs text-muted-foreground">
                  ({value.length})
                </span>
              )}
            </button>
          </div>
          <div className="pt-3">
            {textTab === 'write' ? (
              <Textarea
                placeholder={`Enter ingredients, one per line...\nUse "GroupName:" to create groups.\n\nExamples:\nBiscuit:\n2 cups flour\n1 1/2 tsp salt\nGravy:\n2 T butter\n3 large eggs`}
                className="min-h-[200px] font-mono text-sm"
                value={rawText}
                onChange={(e) => handleTextChange(e.target.value)}
              />
            ) : value.length > 0 ? (
              <div className="rounded-md border bg-muted/30 p-3">
                {(() => {
                  let lastGroup: string | null = null
                  return value.map((ingredient, i) => {
                    const showGroupHeader = ingredient.groupName !== lastGroup && ingredient.groupName !== null
                    const isFirstGroup = lastGroup === null && ingredient.groupName !== null
                    lastGroup = ingredient.groupName
                    return (
                      <div key={i}>
                        {showGroupHeader && (
                          <div
                            className={cn(
                              'mb-1 border-b border-muted pb-0.5 text-xs font-bold uppercase tracking-wide text-foreground/70',
                              isFirstGroup ? 'mt-0' : 'mt-3'
                            )}
                          >
                            {ingredient.groupName}
                          </div>
                        )}
                        <div className="flex items-baseline gap-2 py-0.5 text-sm">
                          <span className="min-w-5 text-right text-xs text-muted-foreground">
                            {i + 1}.
                          </span>
                          {ingredient.amountText && (
                            <span className="font-medium">{ingredient.amountText}</span>
                          )}
                          {ingredient.unit && (
                            <span className="text-muted-foreground">{formatUnit(ingredient.unit, ingredient.amount)}</span>
                          )}
                          {ingredient.name && <span>{ingredient.name}</span>}
                          {!ingredient.amountText && !ingredient.unit && !ingredient.name && (
                            <span className="italic text-muted-foreground">
                              {ingredient.rawText}
                            </span>
                          )}
                        </div>
                      </div>
                    )
                  })
                })()}
              </div>
            ) : (
              <p className="py-8 text-center text-sm text-muted-foreground">
                Nothing to preview yet. Switch to Write and add some ingredients.
              </p>
            )}
          </div>
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
