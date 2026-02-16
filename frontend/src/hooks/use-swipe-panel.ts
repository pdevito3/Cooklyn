import { useCallback, useEffect, useRef, useState } from 'react'

// Vaul-style gesture constants
const VELOCITY_THRESHOLD = 0.4 // px/ms — fast swipe overrides distance
const SNAP_THRESHOLD = 0.35 // fraction of panel width to trigger snap
const SNAP_DURATION = '0.3s'
const SNAP_EASING = 'cubic-bezier(0.32, 0.72, 0, 1)'

interface UseSwipePanelOptions {
  /** Width of the panel as a percentage of viewport (default 85) */
  widthPercent?: number
  /** Pixels from the right edge that start a drag-to-open (default 30) */
  edgeZone?: number
  /** Whether the gesture system is active */
  enabled?: boolean
}

interface SwipePanelRefs {
  panelRef: React.RefObject<HTMLDivElement | null>
  backdropRef: React.RefObject<HTMLDivElement | null>
}

interface SwipePanelHandlers {
  onPointerDown: (e: React.PointerEvent) => void
  onPointerMove: (e: React.PointerEvent) => void
  onPointerUp: (e: React.PointerEvent) => void
}

interface UseSwipePanelReturn {
  isOpen: boolean
  setIsOpen: (open: boolean) => void
  toggle: () => void
  refs: SwipePanelRefs
  handlers: SwipePanelHandlers
  /** Inline styles for the panel element */
  panelStyle: React.CSSProperties
  /** Inline styles for the backdrop element */
  backdropStyle: React.CSSProperties
}

export function useSwipePanel(
  options: UseSwipePanelOptions = {},
): UseSwipePanelReturn {
  const { widthPercent = 85, edgeZone = 30, enabled = true } = options

  const [isOpen, setIsOpen] = useState(false)
  const panelRef = useRef<HTMLDivElement>(null)
  const backdropRef = useRef<HTMLDivElement>(null)

  const dragState = useRef<{
    startX: number
    startY: number
    startTime: number
    dragging: boolean
    locked: boolean
    isHorizontal: boolean
  } | null>(null)

  const panelWidthPx = useCallback(
    () => (window.innerWidth * widthPercent) / 100,
    [widthPercent],
  )

  const setTranslate = useCallback(
    (translateX: number, animate: boolean) => {
      const panel = panelRef.current
      const backdrop = backdropRef.current
      if (!panel) return
      const width = panelWidthPx()
      const clamped = Math.max(0, Math.min(translateX, width))
      const progress = 1 - clamped / width // 1 = fully open, 0 = closed
      const transition = animate
        ? `transform ${SNAP_DURATION} ${SNAP_EASING}`
        : 'none'
      panel.style.transition = transition
      panel.style.transform = `translate3d(${clamped}px, 0, 0)`
      if (backdrop) {
        backdrop.style.transition = animate
          ? `opacity ${SNAP_DURATION} ${SNAP_EASING}`
          : 'none'
        backdrop.style.opacity = String(progress * 0.5)
        backdrop.style.pointerEvents = progress > 0 ? 'auto' : 'none'
      }
    },
    [panelWidthPx],
  )

  const snap = useCallback(
    (open: boolean) => {
      setTranslate(open ? 0 : panelWidthPx(), true)
      setIsOpen(open)
    },
    [setTranslate, panelWidthPx],
  )

  // Sync visual position when isOpen changes externally (button, hotkey)
  useEffect(() => {
    if (!enabled) return
    setTranslate(isOpen ? 0 : panelWidthPx(), true)
  }, [isOpen, enabled, setTranslate, panelWidthPx])

  const onPointerDown = useCallback(
    (e: React.PointerEvent) => {
      if (!enabled) return

      const x = e.clientX
      const nearRightEdge = x >= window.innerWidth - edgeZone

      if (!isOpen && !nearRightEdge) return
      if (isOpen) {
        const panelLeft = window.innerWidth - panelWidthPx()
        if (x < panelLeft - 20) return
      }

      dragState.current = {
        startX: x,
        startY: e.clientY,
        startTime: Date.now(),
        dragging: true,
        locked: false,
        isHorizontal: false,
      }
      ;(e.target as HTMLElement).setPointerCapture(e.pointerId)
    },
    [enabled, isOpen, edgeZone, panelWidthPx],
  )

  const onPointerMove = useCallback(
    (e: React.PointerEvent) => {
      const state = dragState.current
      if (!state?.dragging) return

      const dx = e.clientX - state.startX
      const dy = e.clientY - state.startY

      if (!state.locked && (Math.abs(dx) > 10 || Math.abs(dy) > 10)) {
        state.locked = true
        state.isHorizontal = Math.abs(dx) > Math.abs(dy)
        if (!state.isHorizontal) {
          state.dragging = false
          return
        }
      }

      if (!state.locked || !state.isHorizontal) return

      const width = panelWidthPx()
      const translateX = isOpen ? Math.max(0, dx) : Math.max(0, width + dx)

      setTranslate(translateX, false)
    },
    [isOpen, panelWidthPx, setTranslate],
  )

  const onPointerUp = useCallback(
    (e: React.PointerEvent) => {
      const state = dragState.current
      if (!state?.dragging) {
        dragState.current = null
        // Tap on backdrop closes panel
        if (isOpen) {
          const panelLeft = window.innerWidth - panelWidthPx()
          if (e.clientX < panelLeft) snap(false)
        }
        return
      }

      const dx = e.clientX - state.startX
      const elapsed = Date.now() - state.startTime
      const velocity = Math.abs(dx) / elapsed

      if (!state.locked) {
        dragState.current = null
        if (isOpen) {
          const panelLeft = window.innerWidth - panelWidthPx()
          if (e.clientX < panelLeft) snap(false)
        }
        return
      }

      const width = panelWidthPx()
      const absDx = Math.abs(dx)

      let shouldOpen: boolean
      if (velocity > VELOCITY_THRESHOLD) {
        shouldOpen = dx < 0
      } else {
        shouldOpen = isOpen
          ? absDx < width * SNAP_THRESHOLD
          : absDx > width * SNAP_THRESHOLD
      }

      snap(shouldOpen)
      dragState.current = null
    },
    [isOpen, panelWidthPx, snap],
  )

  const toggle = useCallback(() => setIsOpen((o) => !o), [])

  return {
    isOpen,
    setIsOpen,
    toggle,
    refs: { panelRef, backdropRef },
    handlers: { onPointerDown, onPointerMove, onPointerUp },
    panelStyle: {
      width: `${widthPercent}%`,
      transform: 'translate3d(100%, 0, 0)',
    },
    backdropStyle: {
      opacity: 0,
      pointerEvents: 'none' as const,
    },
  }
}
