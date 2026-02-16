import { useCallback, useRef, useState } from 'react'
import { cn } from '@/lib/utils'

interface StepViewerProps {
  steps: string
}

export function parseSteps(text: string): string[] {
  return text
    .split(/\n\n+/)
    .map((s) => s.trim())
    .filter(Boolean)
}

export function stripLeadingNumber(text: string): string {
  return text.replace(/^\d+\.\s*/, '')
}

export function StepViewer({ steps }: StepViewerProps) {
  const [activeStep, setActiveStep] = useState<number | null>(null)
  const stepRefs = useRef<Map<number, HTMLDivElement>>(new Map())

  const handleStepClick = useCallback((index: number) => {
    setActiveStep((prev) => (prev === index ? null : index))
    const el = stepRefs.current.get(index)
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'nearest' })
    }
  }, [])

  const parsed = parseSteps(steps)

  if (parsed.length === 0) return null

  return (
    <div className="space-y-0">
      {parsed.map((step, index) => {
        const isActive = activeStep === index
        const isLast = index === parsed.length - 1

        return (
          <div
            key={index}
            ref={(el) => {
              if (el) stepRefs.current.set(index, el)
              else stepRefs.current.delete(index)
            }}
            className="group flex cursor-pointer gap-4"
            onClick={() => handleStepClick(index)}
            role="button"
            tabIndex={0}
            onKeyDown={(e) => {
              if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault()
                handleStepClick(index)
              }
            }}
          >
            {/* Timeline column */}
            <div className="flex flex-col items-center">
              {/* Circle */}
              <div
                className={cn(
                  'flex h-8 w-8 shrink-0 items-center justify-center rounded-full border-2 text-sm font-semibold transition-colors',
                  isActive
                    ? 'border-primary bg-primary text-primary-foreground'
                    : 'border-muted-foreground/30 bg-background text-muted-foreground group-hover:border-primary/50'
                )}
              >
                {index + 1}
              </div>
              {/* Connecting line */}
              {!isLast && (
                <div className="w-0.5 grow bg-muted-foreground/20" />
              )}
            </div>

            {/* Step content */}
            <div
              className={cn(
                'pb-6 pt-1 transition-colors',
                isActive ? 'text-foreground' : 'text-muted-foreground group-hover:text-foreground/80'
              )}
            >
              <p className={cn('whitespace-pre-wrap text-sm leading-relaxed', isActive && 'font-medium')}>
                {stripLeadingNumber(step)}
              </p>
            </div>
          </div>
        )
      })}
    </div>
  )
}
