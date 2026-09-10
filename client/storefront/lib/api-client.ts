import axios from 'axios';

const publicApiUrl = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5101/api';

/**
 * The browser and the server reach the API at different addresses once a proxy
 * sits in front: the browser uses the public origin, while server components
 * run inside the container network, where that origin resolves to the
 * container's own loopback and the request is refused.
 *
 * INTERNAL_API_ORIGIN (no NEXT_PUBLIC_ prefix, so it stays server-side and is
 * read at request time) names the in-network address. Unset, this falls back to
 * the public URL, which is correct when there is no proxy — `next dev` on a
 * workstation, or a deployment where both addresses are the same.
 */
function resolveBaseUrl(): string {
  if (typeof window !== 'undefined') return publicApiUrl;

  const internalOrigin = process.env.INTERNAL_API_ORIGIN;
  return internalOrigin ? `${internalOrigin.replace(/\/$/, '')}/api` : publicApiUrl;
}

const apiClient = axios.create({
  baseURL: resolveBaseUrl(),
  headers: {
    'Content-Type': 'application/json',
  },
  // ASP.NET binds a List<T> from repeated keys (`brandIds=1&brandIds=2`).
  // Axios defaults to `brandIds[]=1`, which the model binder ignores outright —
  // the filter then silently does nothing instead of failing.
  paramsSerializer: { indexes: null },
});

export default apiClient;
