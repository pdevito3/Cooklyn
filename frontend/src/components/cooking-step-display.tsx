import {
  ArrowLeft02Icon,
  ArrowRight02Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { useEffect, useRef } from 'react'
import { useHotkeys } from 'react-hotkeys-hook'

import { Button } from '@/components/ui/button'
import { Kbd } from '@/components/ui/kbd'
import { cn } from '@/lib/utils'

interface CookingStepDisplayProps {
  steps: string[]
  currentStep: number
  onStepChange: (step: number) => void
}

export function CookingStepDisplay({
  steps,
  currentStep,
  onStepChange,
}: CookingStepDisplayProps) {
  const stepRefs = useRef<Map<number, HTMLDivElement>>(new Map())

  const canGoPrev = currentStep > 0
  const canGoNext = currentStep < steps.length - 1

  const goPrev = () => {
    if (canGoPrev) onStepChange(currentStep - 1)
  }

  const goNext = () => {
    if (canGoNext) onStepChange(currentStep + 1)
  }

  useHotkeys('left', goPrev, [currentStep, canGoPrev])
  useHotkeys('right', goNext, [currentStep, canGoNext])

  useEffect(() => {
    const el = stepRefs.current.get(currentStep)
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'nearest' })
    }
  }, [currentStep])

  return (
    <div className="flex flex-1 flex-col px-4 py-4 md:px-8">
      {/* Scrollable step list */}
      <div className="flex-1 overflow-y-auto">
        <div className="mx-auto max-w-2xl space-y-0">
          {steps.map((step, index) => {
            const isActive = index === currentStep
            const isLast = index === steps.length - 1

            return (
              <div
                key={index}
                ref={(el) => {
                  if (el) stepRefs.current.set(index, el)
                  else stepRefs.current.delete(index)
                }}
                className="group flex cursor-pointer gap-4"
                onClick={() => onStepChange(index)}
                role="button"
                tabIndex={0}
                onKeyDown={(e) => {
                  if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault()
                    onStepChange(index)
                  }
                }}
              >
                {/* Timeline column */}
                <div className="flex flex-col items-center">
                  <div
                    className={cn(
                      'flex h-8 w-8 shrink-0 items-center justify-center rounded-full border-2 text-sm font-semibold transition-colors',
                      isActive
                        ? 'border-primary bg-primary text-primary-foreground'
                        : 'border-muted-foreground/30 bg-background text-muted-foreground group-hover:border-primary/50',
                    )}
                  >
                    {index + 1}
                  </div>
                  {!isLast && (
                    <div className="w-0.5 grow bg-muted-foreground/20" />
                  )}
                </div>

                {/* Step content */}
                <div
                  className={cn(
                    'pb-6 pt-1 transition-colors',
                    isActive
                      ? 'text-foreground'
                      : 'text-muted-foreground group-hover:text-foreground/80',
                  )}
                >
                  <p
                    className={cn(
                      'whitespace-pre-wrap leading-relaxed',
                      isActive
                        ? 'text-base font-medium md:text-lg'
                        : 'text-sm',
                    )}
                  >
                    {step}
                  </p>
                </div>
              </div>
            )
          })}
        </div>
      </div>

      {/* Prev / Next buttons */}
      <div className="mx-auto flex w-full max-w-md items-center justify-between gap-4 pt-4">
        <Button variant="outline" onClick={goPrev} disabled={!canGoPrev}>
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-4 w-4" />
          Previous
          <Kbd>&larr;</Kbd>
        </Button>
        <span className="text-sm text-muted-foreground">
          {currentStep + 1} / {steps.length}
        </span>
        <Button variant="outline" onClick={goNext} disabled={!canGoNext}>
          Next
          <HugeiconsIcon icon={ArrowRight02Icon} className="h-4 w-4" />
          <Kbd>&rarr;</Kbd>
        </Button>
      </div>
    </div>
  )
}
