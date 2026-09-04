"use client"

import { useSyncExternalStore } from "react"
import { useQuery } from "@tanstack/react-query"
import { getCart } from "../api/cart.api"
import { CART_QUERY_KEYS } from "../constants"
import { getCartSessionId, subscribeToCartSession } from "@/lib/cart-session"

export const cartKeys = {
  all: CART_QUERY_KEYS.ALL,
  active: (sessionId: string | null = getCartSessionId()) =>
    [...cartKeys.all, sessionId] as const,
}

/** The server has no localStorage, so it — and the hydrating render — see null. */
const getServerSnapshot = () => null

/**
 * Reads the guest cart session as an external store. Reading localStorage
 * during render would make the server and the first client render disagree,
 * which is a hydration error; this returns null until hydration finishes and
 * then re-renders with the real id.
 */
export function useCartSessionId(): string | null {
  return useSyncExternalStore(
    subscribeToCartSession,
    getCartSessionId,
    getServerSnapshot,
  )
}

export function useCart(enabled = true) {
  const sessionId = useCartSessionId()

  return useQuery({
    queryKey: cartKeys.active(sessionId),
    queryFn: getCart,
    staleTime: 30_000,
    enabled: enabled && sessionId != null,
  })
}

export function useCartCount(): number {
  const { data } = useCart()
  return data?.itemCount ?? 0
}
