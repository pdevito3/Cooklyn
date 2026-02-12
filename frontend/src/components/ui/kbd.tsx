export function Kbd({ children }: { children: React.ReactNode }) {
  return (
    <kbd className="pointer-events-none ml-2 hidden shrink-0 select-none items-center justify-center rounded border bg-muted/50 px-1.5 py-0.5 font-sans text-xs font-semibold text-muted-foreground sm:inline-flex">
      {children}
    </kbd>
  );
}
