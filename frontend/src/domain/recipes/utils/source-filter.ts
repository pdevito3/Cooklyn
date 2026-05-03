export function getRecipeSourceDomain(source: string | null | undefined): string | null {
  if (!source) return null
  try {
    const url = new URL(source)
    if (url.protocol !== 'http:' && url.protocol !== 'https:') return null
    const host = url.hostname.replace(/^www\./, '')
    return host || null
  } catch {
    return null
  }
}

export function buildSourceDomainFilter(domain: string): string {
  const escaped = domain.replace(/"/g, '\\"')
  return `source @=* "${escaped}"`
}
