import * as React from 'react'

import { cn } from '@/lib/utils'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { InputGroup, InputGroupAddon } from '@/components/ui/input-group'
import { Search01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

// --- Context ---

interface CommandContextType {
  search: string
  setSearch: (s: string) => void
  shouldFilter: boolean
  activeValue: string
  setActiveValue: (v: string) => void
  rootRef: React.RefObject<HTMLDivElement | null>
}

const CommandContext = React.createContext<CommandContextType | null>(null)

function useCommand() {
  const ctx = React.useContext(CommandContext)
  if (!ctx) throw new Error('useCommand must be used within <Command>')
  return ctx
}

// --- Helpers ---

function getSelectableItems(root: HTMLElement | null): HTMLElement[] {
  if (!root) return []
  return Array.from(
    root.querySelectorAll<HTMLElement>(
      '[data-command-item]:not([data-disabled="true"])',
    ),
  )
}

// --- Command ---

interface CommandProps extends React.ComponentProps<'div'> {
  shouldFilter?: boolean
}

function Command({
  shouldFilter = true,
  className,
  children,
  ...props
}: CommandProps) {
  const [search, setSearch] = React.useState('')
  const [activeValue, setActiveValue] = React.useState('')
  const rootRef = React.useRef<HTMLDivElement>(null)

  // Keep active value valid when items change (intentionally runs every render
  // to catch DOM changes from filtering/data loading — terminates in max 2 cycles)
  // oxlint-disable-next-line react-hooks/exhaustive-deps
  React.useLayoutEffect(() => {
    const items = getSelectableItems(rootRef.current)
    if (items.length === 0) {
      if (activeValue !== '') setActiveValue('')
      return
    }
    const isValid = items.some(
      (el) => el.getAttribute('data-value') === activeValue,
    )
    if (!isValid) {
      setActiveValue(items[0].getAttribute('data-value') ?? '')
    }
  })

  const handleKeyDown = React.useCallback(
    (e: React.KeyboardEvent) => {
      const items = getSelectableItems(rootRef.current)
      if (!items.length) return

      if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
        e.preventDefault()
        const idx = items.findIndex(
          (el) => el.getAttribute('data-value') === activeValue,
        )
        const next =
          e.key === 'ArrowDown'
            ? idx < items.length - 1
              ? idx + 1
              : 0
            : idx > 0
              ? idx - 1
              : items.length - 1
        const nextValue = items[next].getAttribute('data-value') ?? ''
        setActiveValue(nextValue)
        items[next].scrollIntoView({ block: 'nearest' })
      } else if (e.key === 'Enter') {
        e.preventDefault()
        const el = items.find(
          (el) => el.getAttribute('data-value') === activeValue,
        )
        el?.click()
      } else if (e.key === 'Home') {
        e.preventDefault()
        setActiveValue(items[0].getAttribute('data-value') ?? '')
        items[0].scrollIntoView({ block: 'nearest' })
      } else if (e.key === 'End') {
        e.preventDefault()
        const last = items[items.length - 1]
        setActiveValue(last.getAttribute('data-value') ?? '')
        last.scrollIntoView({ block: 'nearest' })
      }
    },
    [activeValue],
  )

  const ctx = React.useMemo(
    () => ({
      search,
      setSearch,
      shouldFilter,
      activeValue,
      setActiveValue,
      rootRef,
    }),
    [search, shouldFilter, activeValue],
  )

  return (
    <CommandContext.Provider value={ctx}>
      <div
        ref={rootRef}
        data-slot="command"
        className={cn(
          'bg-popover text-popover-foreground flex size-full flex-col overflow-hidden rounded-xl p-1',
          className,
        )}
        onKeyDown={handleKeyDown}
        tabIndex={-1}
        {...props}
      >
        {children}
      </div>
    </CommandContext.Provider>
  )
}

// --- CommandDialog ---

function CommandDialog({
  title = 'Command Palette',
  description = 'Search for a command to run...',
  children,
  className,
  ...props
}: Omit<React.ComponentProps<typeof Dialog>, 'children'> & {
  title?: string
  description?: string
  className?: string
  children: React.ReactNode
}) {
  return (
    <Dialog {...props}>
      <DialogHeader className="sr-only">
        <DialogTitle>{title}</DialogTitle>
        <DialogDescription>{description}</DialogDescription>
      </DialogHeader>
      <DialogContent
        className={cn(
          'top-1/3 translate-y-0 overflow-hidden rounded-xl p-0',
          className,
        )}
      >
        {children}
      </DialogContent>
    </Dialog>
  )
}

// --- CommandInput ---

interface CommandInputProps
  extends Omit<React.ComponentProps<'input'>, 'value' | 'onChange'> {
  value?: string
  onValueChange?: (value: string) => void
}

function CommandInput({
  className,
  value,
  onValueChange,
  ...props
}: CommandInputProps) {
  const { search, setSearch } = useCommand()
  const inputRef = React.useRef<HTMLInputElement>(null)

  // Autofocus on mount
  React.useEffect(() => {
    const frame = requestAnimationFrame(() => inputRef.current?.focus())
    return () => cancelAnimationFrame(frame)
  }, [])

  const isControlled = value !== undefined
  const displayValue = isControlled ? value : search

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const v = e.target.value
    setSearch(v)
    onValueChange?.(v)
  }

  return (
    <div data-slot="command-input-wrapper" className="p-1 pb-0">
      <InputGroup className="bg-input/30 border-input/30 h-8! rounded-lg! shadow-none! *:data-[slot=input-group-addon]:pl-2!">
        <InputGroupAddon>
          <HugeiconsIcon
            icon={Search01Icon}
            className="size-4 shrink-0 opacity-50"
          />
        </InputGroupAddon>
        <input
          ref={inputRef}
          data-slot="command-input"
          className={cn(
            'w-full bg-transparent text-sm outline-hidden disabled:cursor-not-allowed disabled:opacity-50',
            className,
          )}
          value={displayValue}
          onChange={handleChange}
          {...props}
        />
      </InputGroup>
    </div>
  )
}

// --- CommandList ---

function CommandList({ className, ...props }: React.ComponentProps<'div'>) {
  return (
    <div
      data-slot="command-list"
      className={cn(
        'no-scrollbar max-h-96 scroll-py-1 overflow-x-hidden overflow-y-auto outline-none',
        className,
      )}
      {...props}
    />
  )
}

// --- CommandEmpty ---

function CommandEmpty({ className, ...props }: React.ComponentProps<'div'>) {
  return (
    <div
      data-slot="command-empty"
      className={cn('py-6 text-center text-sm', className)}
      {...props}
    />
  )
}

// --- CommandGroup ---

interface CommandGroupProps extends React.ComponentProps<'div'> {
  heading?: string
}

function CommandGroup({
  className,
  heading,
  children,
  ...props
}: CommandGroupProps) {
  return (
    <div
      data-slot="command-group"
      className={cn(
        'overflow-hidden p-1',
        // Auto-hide when all children are filtered out
        'hidden has-[[data-command-item]]:block',
        className,
      )}
      {...props}
    >
      {heading && (
        <div className="text-muted-foreground px-2 py-1.5 text-xs font-semibold">
          {heading}
        </div>
      )}
      {children}
    </div>
  )
}

// --- CommandSeparator ---

function CommandSeparator({
  className,
  ...props
}: React.ComponentProps<'div'>) {
  return (
    <div
      data-slot="command-separator"
      className={cn('bg-border -mx-1 h-px', className)}
      {...props}
    />
  )
}

// --- CommandItem ---

interface CommandItemProps
  extends Omit<React.ComponentProps<'div'>, 'onSelect'> {
  value?: string
  disabled?: boolean
  onSelect?: (value: string) => void
  keywords?: string[]
}

function CommandItem({
  className,
  children,
  value = '',
  disabled = false,
  onSelect,
  keywords,
  ...props
}: CommandItemProps) {
  const { search, shouldFilter, activeValue, setActiveValue } = useCommand()
  const ref = React.useRef<HTMLDivElement>(null)
  const textRef = React.useRef('')

  // Capture text content for filtering
  React.useLayoutEffect(() => {
    if (ref.current) {
      textRef.current = ref.current.textContent?.toLowerCase() ?? ''
    }
  })

  const isMatch = React.useMemo(() => {
    if (!shouldFilter || !search) return true
    const s = search.toLowerCase()
    if (value.toLowerCase().includes(s)) return true
    if (textRef.current.includes(s)) return true
    if (keywords?.some((kw) => kw.toLowerCase().includes(s))) return true
    return false
  }, [shouldFilter, search, value, keywords])

  if (!isMatch) return null

  const isActive = activeValue === value

  return (
    <div
      ref={ref}
      data-slot="command-item"
      data-command-item=""
      data-value={value}
      data-disabled={disabled || undefined}
      data-active={isActive || undefined}
      className={cn(
        "group/command-item relative flex cursor-default items-center gap-2 rounded-md px-2 py-1.5 text-sm outline-hidden select-none",
        "data-[active]:bg-accent data-[active]:text-accent-foreground",
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
        "[&_svg:not([class*='size-'])]:size-4 [&_svg]:pointer-events-none [&_svg]:shrink-0",
        className,
      )}
      onClick={() => {
        if (!disabled) onSelect?.(value)
      }}
      onPointerMove={() => {
        if (!disabled && activeValue !== value) setActiveValue(value)
      }}
      {...props}
    >
      {children}
    </div>
  )
}

// --- CommandShortcut ---

function CommandShortcut({
  className,
  ...props
}: React.ComponentProps<'span'>) {
  return (
    <span
      data-slot="command-shortcut"
      className={cn(
        'text-muted-foreground ml-auto text-xs tracking-widest',
        className,
      )}
      {...props}
    />
  )
}

export {
  Command,
  CommandDialog,
  CommandInput,
  CommandList,
  CommandEmpty,
  CommandGroup,
  CommandItem,
  CommandShortcut,
  CommandSeparator,
}
