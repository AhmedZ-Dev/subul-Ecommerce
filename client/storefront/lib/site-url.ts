/**
 * Public origin of the storefront. Open Graph and canonical URLs must be
 * absolute — a relative path gives WhatsApp and Google nothing to fetch.
 */
export function getSiteUrl(): string {
  const configured = process.env.NEXT_PUBLIC_SITE_URL?.trim()
  if (configured) return configured.replace(/\/+$/, "")
  return "http://localhost:3001"
}

/** Absolute URL for a route path within the storefront. */
export function absoluteUrl(path: string): string {
  return `${getSiteUrl()}${path.startsWith("/") ? path : `/${path}`}`
}
