import { useState } from 'react'

import { ScaleInput, formatScaledAmount } from '@/components/scale-input'
import { formatUnit } from '@/domain/recipes/types'
import type { IngredientDto } from '@/domain/recipes/types'
import { cn } from '@/lib/utils'

interface CookingIngredientsPanelProps {
  ingredients: IngredientDto[]
}

export function CookingIngredientsPanel({
  ingredients,
}: CookingIngredientsPanelProps) {
  const [scale, setScale] = useState(1)

  const sorted = [...ingredients].toSorted((a, b) => a.sortOrder - b.sortOrder)
  let lastGroup: string | null | undefined = undefined

  return (
    <div className="flex h-full flex-col">
      <div className="flex items-center justify-between border-b px-4 py-3">
        <h3 className="text-sm font-semibold">Ingredients</h3>
        <ScaleInput value={scale} onChange={setScale} />
      </div>
      <div className="flex-1 overflow-y-auto px-4 py-3">
        <div className="space-y-1">
          {sorted.map((ingredient) => {
            const showGroupHeader =
              ingredient.groupName !== lastGroup &&
              ingredient.groupName !== null
            const isFirstGroup = lastGroup === undefined
            lastGroup = ingredient.groupName

            const scaledAmount =
              ingredient.amount != null
                ? formatScaledAmount(ingredient.amount * scale)
                : null

            return (
              <div key={ingredient.id}>
                {showGroupHeader && (
                  <h4
                    className={cn(
                      'border-b pb-1 text-sm font-semibold uppercase tracking-wide text-foreground/70',
                      isFirstGroup ? 'mt-0 mb-2' : 'mt-4 mb-2',
                    )}
                  >
                    {ingredient.groupName}
                  </h4>
                )}
                <li className="flex gap-1 list-none text-sm">
                  {scaledAmount != null ? (
                    <span
                      className={cn(
                        'font-medium',
                        scale !== 1 && 'text-primary',
                      )}
                    >
                      {scaledAmount}
                    </span>
                  ) : ingredient.amountText ? (
                    <span className="font-medium">{ingredient.amountText}</span>
                  ) : null}
                  {ingredient.unit && (
                    <span className="text-muted-foreground">
                      {formatUnit(
                        ingredient.unit,
                        ingredient.amount != null
                          ? ingredient.amount * scale
                          : null,
                      )}
                    </span>
                  )}
                  <span>{ingredient.name ?? ingredient.rawText}</span>
                </li>
              </div>
            )
          })}
        </div>
      </div>
    </div>
  )
}
