interface CookingProgressBarProps {
  currentStep: number
  totalSteps: number
}

export function CookingProgressBar({
  currentStep,
  totalSteps,
}: CookingProgressBarProps) {
  const progress = ((currentStep + 1) / totalSteps) * 100

  return (
    <div className="h-1 w-full bg-muted">
      <div
        className="h-full bg-primary transition-all duration-300 ease-in-out"
        style={{ width: `${progress}%` }}
      />
    </div>
  )
}
