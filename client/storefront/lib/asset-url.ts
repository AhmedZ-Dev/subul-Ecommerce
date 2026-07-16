export function getApiOrigin(): string {
  const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5101/api"
  return apiUrl.replace(/\/api\/?$/, "")
}

/** Resolves a stored asset path (e.g. /img/products/1/abc.png) to a full URL. */
export function resolveAssetUrl(path: string | null | undefined): string | null {
  if (!path) return null
  if (path.startsWith("http://") || path.startsWith("https://")) return path
  const origin = getApiOrigin()
  return `${origin}${path.startsWith("/") ? path : `/${path}`}`
}
