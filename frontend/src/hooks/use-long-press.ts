import { useCallback, useRef } from 'react'

interface UseLongPressOptions {
  onLongPress: () => void
  thresholdMs?: number
  moveTolerancePx?: number
}

export function useLongPress({
  onLongPress,
  thresholdMs = 500,
  moveTolerancePx = 8,
}: UseLongPressOptions) {
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null)
  const startRef = useRef<{ x: number; y: number } | null>(null)
  const firedRef = useRef(false)

  const clear = useCallback(() => {
    if (timerRef.current) {
      clearTimeout(timerRef.current)
      timerRef.current = null
    }
  }, [])

  const onTouchStart = useCallback(
    (e: React.TouchEvent) => {
      const touch = e.touches[0]
      if (!touch) return
      startRef.current = { x: touch.clientX, y: touch.clientY }
      firedRef.current = false
      clear()
      timerRef.current = setTimeout(() => {
        firedRef.current = true
        onLongPress()
      }, thresholdMs)
    },
    [clear, onLongPress, thresholdMs],
  )

  const onTouchMove = useCallback(
    (e: React.TouchEvent) => {
      const touch = e.touches[0]
      const start = startRef.current
      if (!touch || !start) return
      const dx = touch.clientX - start.x
      const dy = touch.clientY - start.y
      if (dx * dx + dy * dy > moveTolerancePx * moveTolerancePx) {
        clear()
      }
    },
    [clear, moveTolerancePx],
  )

  const onTouchEnd = useCallback(
    (e: React.TouchEvent) => {
      clear()
      if (firedRef.current) {
        e.preventDefault()
        firedRef.current = false
      }
    },
    [clear],
  )

  const onTouchCancel = useCallback(() => {
    clear()
    firedRef.current = false
  }, [clear])

  return {
    onTouchStart,
    onTouchMove,
    onTouchEnd,
    onTouchCancel,
  }
}
