import { Cancel01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { useEffect, useState } from 'react'
import { useHotkeys } from 'react-hotkeys-hook'

import { CookingIngredientsPanel } from '@/components/cooking-ingredients-panel'
import { CookingProgressBar } from '@/components/cooking-progress-bar'
import { CookingStepDisplay } from '@/components/cooking-step-display'
import { parseSteps, stripLeadingNumber } from '@/components/step-viewer'
import { Button } from '@/components/ui/button'
import { Kbd } from '@/components/ui/kbd'
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from '@/components/ui/sheet'
import type { IngredientDto } from '@/domain/recipes'
import { useIsMobile } from '@/hooks/use-mobile'

interface CookingViewProps {
  title: string
  steps: string
  ingredients: IngredientDto[]
  onClose: () => void
}

export function CookingView({
  title,
  steps,
  ingredients,
  onClose,
}: CookingViewProps) {
  const isMobile = useIsMobile()
  const [currentStep, setCurrentStep] = useState(0)
  const [ingredientsOpen, setIngredientsOpen] = useState(!isMobile)

  const parsed = parseSteps(steps).map(stripLeadingNumber)
  const hasIngredients = ingredients.length > 0

  // Body scroll lock
  useEffect(() => {
    const prev = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    return () => {
      document.body.style.overflow = prev
    }
  }, [])

  useHotkeys('escape', onClose)
  useHotkeys(
    'i',
    () => {
      if (hasIngredients) setIngredientsOpen((o) => !o)
    },
    [hasIngredients],
  )

  if (parsed.length === 0) return null

  return (
    <div className="fixed inset-0 z-50 flex flex-col bg-background">
      {/* Progress bar */}
      <CookingProgressBar currentStep={currentStep} totalSteps={parsed.length} />

      {/* Top bar */}
      <div className="flex items-center justify-between border-b px-4 py-2">
        <h2 className="truncate text-sm font-medium">{title}</h2>
        <div className="flex items-center gap-2">
          {hasIngredients && (
            <Button
              variant="outline"
              size="sm"
              onClick={() => setIngredientsOpen((o) => !o)}
            >
              Ingredients
              <Kbd>I</Kbd>
            </Button>
          )}
          <Button variant="ghost" size="icon-sm" onClick={onClose}>
            <HugeiconsIcon icon={Cancel01Icon} />
            <span className="sr-only">Close</span>
          </Button>
        </div>
      </div>

      {/* Main content */}
      <div className="flex flex-1 overflow-hidden">
        <CookingStepDisplay
          steps={parsed}
          currentStep={currentStep}
          onStepChange={setCurrentStep}
        />

        {/* Desktop inline ingredients panel */}
        {!isMobile && hasIngredients && ingredientsOpen && (
          <div className="w-80 border-l">
            <CookingIngredientsPanel ingredients={ingredients} />
          </div>
        )}
      </div>

      {/* Mobile ingredients sheet */}
      {isMobile && hasIngredients && (
        <Sheet open={ingredientsOpen} onOpenChange={setIngredientsOpen}>
          <SheetContent side="right" className="w-[85%] sm:max-w-[85%] p-0">
            <SheetHeader className="sr-only">
              <SheetTitle>Ingredients</SheetTitle>
            </SheetHeader>
            <CookingIngredientsPanel ingredients={ingredients} />
          </SheetContent>
        </Sheet>
      )}
    </div>
  )
}
