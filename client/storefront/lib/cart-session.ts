const KEY = "cart_session_id"

/** Fired when this tab changes the session so subscribers re-read it. */
const CHANGE_EVENT = "subul:cart-session-change"

export function getCartSessionId(): string | null {
  if (typeof window === "undefined") return null
  return localStorage.getItem(KEY)
}

export function setCartSessionId(id: string): void {
  if (typeof window === "undefined") return
  localStorage.setItem(KEY, id)
  notifyChange()
}

export function clearCartSessionId(): void {
  if (typeof window === "undefined") return
  localStorage.removeItem(KEY)
  notifyChange()
}

/**
 * Subscribes to session changes — the "storage" event covers other tabs, the
 * custom event covers this one, since `storage` does not fire on the writer.
 */
export function subscribeToCartSession(onChange: () => void): () => void {
  if (typeof window === "undefined") return () => {}

  window.addEventListener("storage", onChange)
  window.addEventListener(CHANGE_EVENT, onChange)

  return () => {
    window.removeEventListener("storage", onChange)
    window.removeEventListener(CHANGE_EVENT, onChange)
  }
}

function notifyChange(): void {
  window.dispatchEvent(new Event(CHANGE_EVENT))
}
