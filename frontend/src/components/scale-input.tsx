import { useCallback, useState } from 'react'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'

interface ScaleInputProps {
  value: number
  onChange: (value: number) => void
}

export function ScaleInput({ value, onChange }: ScaleInputProps) {
  const [inputText, setInputText] = useState(value.toString())

  const handleChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const raw = e.target.value
      if (raw === '' || /^\d*\.?\d*$/.test(raw)) {
        setInputText(raw)
        const parsed = parseFloat(raw)
        if (!isNaN(parsed) && parsed > 0) onChange(parsed)
      }
    },
    [onChange],
  )

  const handleBlur = useCallback(() => {
    const parsed = parseFloat(inputText)
    if (isNaN(parsed) || parsed <= 0) {
      setInputText(value.toString())
    }
  }, [inputText, value])

  const handleReset = useCallback(() => {
    onChange(1)
    setInputText('1')
  }, [onChange])

  return (
    <div className="flex items-center gap-2">
      <span className="text-sm text-muted-foreground">Scale</span>
      <Input
        type="text"
        inputMode="decimal"
        value={inputText}
        onChange={handleChange}
        onBlur={handleBlur}
        className="w-16 text-center"
      />
      <span className="text-sm text-muted-foreground">x</span>
      {value !== 1 && (
        <Button
          variant="ghost"
          size="sm"
          className="h-7 px-2 text-xs"
          onClick={handleReset}
        >
          Reset
        </Button>
      )}
    </div>
  )
}

// Common fractions for friendly display
const FRACTIONS: [number, string][] = [
  [0.125, '\u215B'], // ⅛
  [0.25, '\u00BC'], // ¼
  [1 / 3, '\u2153'], // ⅓
  [0.375, '\u215C'], // ⅜
  [0.5, '\u00BD'], // ½
  [0.625, '\u215D'], // ⅝
  [2 / 3, '\u2154'], // ⅔
  [0.75, '\u00BE'], // ¾
  [0.875, '\u215E'], // ⅞
]

export function formatScaledAmount(value: number): string {
  if (Number.isInteger(value)) return value.toString()

  const whole = Math.floor(value)
  const remainder = value - whole

  for (const [decimal, glyph] of FRACTIONS) {
    if (Math.abs(remainder - decimal) < 0.01) {
      return whole > 0 ? `${whole}${glyph}` : glyph
    }
  }

  const rounded = Math.round(value * 100) / 100
  return rounded.toString()
}
