export function formatSourceDisplay(source: string): string {
  try {
    const url = new URL(source)
    return url.hostname.replace(/^www\./, '')
  } catch {
    return source.length > 40 ? source.slice(0, 40) + '...' : source
  }
}

export function isSourceUrl(source: string): boolean {
  try {
    new URL(source)
    return true
  } catch {
    return false
  }
}
