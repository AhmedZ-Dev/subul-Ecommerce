export function getApiOrigin(): string {
  const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5101/api"
  return apiUrl.replace(/\/api\/?$/, "")
}

/**
 * Resolves a stored asset path to an ABSOLUTE URL.
 *
 * Use this only where the URL leaves the page and has to stand on its own —
 * Open Graph tags and JSON-LD, which crawlers fetch from outside. For anything
 * rendered by next/image use {@link resolveAssetPath} instead: an absolute URL
 * makes next/image treat the file as remote and refetch it from the server side,
 * where the public origin is not reachable.
 */
export function resolveAssetUrl(path: string | null | undefined): string | null {
  if (!path) return null
  if (path.startsWith("http://") || path.startsWith("https://")) return path
  const origin = getApiOrigin()
  return `${origin}${path.startsWith("/") ? path : `/${path}`}`
}

/**
 * Resolves a stored asset path to a SAME-ORIGIN path for next/image.
 *
 * Returns the path as-is (e.g. /img/products/1/abc.png). The browser then
 * requests it from the storefront's own origin, and the /img rewrite in
 * next.config.ts forwards it to the API — including for the image optimizer,
 * which runs server-side and cannot reach the public origin from inside the
 * container network.
 *
 * Absolute URLs are passed through untouched, so genuinely external images
 * still work through remotePatterns.
 */
export function resolveAssetPath(path: string | null | undefined): string | null {
  if (!path) return null
  if (path.startsWith("http://") || path.startsWith("https://")) return path
  return path.startsWith("/") ? path : `/${path}`
}
