export function Kbd({ children }: { children: React.ReactNode }) {
  return (
    <kbd className="pointer-events-none ml-1.5 hidden select-none items-center rounded border bg-muted/50 px-1 font-mono text-[10px] font-medium text-muted-foreground sm:inline-flex">
      {children}
    </kbd>
  )
}
