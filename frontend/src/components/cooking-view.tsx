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
import type { IngredientDto } from '@/domain/recipes/types'
import { useIsMobile } from '@/hooks/use-mobile'
import { useSwipePanel } from '@/hooks/use-swipe-panel'

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
  const [desktopIngredientsOpen, setDesktopIngredientsOpen] =
    useState(!isMobile)

  const parsed = parseSteps(steps).map(stripLeadingNumber)
  const hasIngredients = ingredients.length > 0

  const swipePanel = useSwipePanel({
    enabled: isMobile && hasIngredients,
  })

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
      if (!hasIngredients) return
      if (isMobile) {
        swipePanel.toggle()
      } else {
        setDesktopIngredientsOpen((o) => !o)
      }
    },
    [hasIngredients, isMobile],
  )

  const handleIngredientsToggle = () => {
    if (isMobile) {
      swipePanel.toggle()
    } else {
      setDesktopIngredientsOpen((o) => !o)
    }
  }

  if (parsed.length === 0) return null

  return (
    <div
      className="fixed inset-0 z-50 flex flex-col bg-background"
      onPointerDown={swipePanel.handlers.onPointerDown}
      onPointerMove={swipePanel.handlers.onPointerMove}
      onPointerUp={swipePanel.handlers.onPointerUp}
      style={{ touchAction: 'pan-y' }}
    >
      {/* Progress bar */}
      <CookingProgressBar
        currentStep={currentStep}
        totalSteps={parsed.length}
      />

      {/* Top bar */}
      <div className="flex items-center justify-between border-b px-4 py-2">
        <h2 className="truncate text-sm font-medium">{title}</h2>
        <div className="flex items-center gap-2">
          {hasIngredients && (
            <Button
              variant="outline"
              size="sm"
              onClick={handleIngredientsToggle}
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
        {!isMobile && hasIngredients && desktopIngredientsOpen && (
          <div className="w-80 border-l">
            <CookingIngredientsPanel ingredients={ingredients} />
          </div>
        )}
      </div>

      {/* Mobile gesture-driven ingredients panel */}
      {isMobile && hasIngredients && (
        <>
          {/* Backdrop */}
          <div
            ref={swipePanel.refs.backdropRef}
            className="fixed inset-0 z-50 bg-black"
            style={swipePanel.backdropStyle}
          />
          {/* Sliding panel with handle */}
          <div
            ref={swipePanel.refs.panelRef}
            className="fixed inset-y-0 right-0 z-50 flex border-l bg-background"
            style={swipePanel.panelStyle}
          >
            {/* Grab handle — sits to the left of the panel content */}
            <div className="flex w-6 shrink-0 items-center justify-center">
              <div className="h-8 w-1 rounded-full bg-muted-foreground/40" />
            </div>
            <div className="flex-1 overflow-hidden">
              <CookingIngredientsPanel ingredients={ingredients} />
            </div>
          </div>
        </>
      )}
    </div>
  )
}
