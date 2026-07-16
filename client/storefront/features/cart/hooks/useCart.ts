"use client"

import { useQuery } from "@tanstack/react-query"
import { getCart } from "../api/cart.api"
import { CART_QUERY_KEYS } from "../constants"
import { getCartSessionId } from "@/lib/cart-session"

export const cartKeys = {
  all: CART_QUERY_KEYS.ALL,
  active: () => [...cartKeys.all, getCartSessionId()] as const,
}

export function useCart(enabled = true) {
  const hasSession = typeof window !== "undefined" && !!getCartSessionId()

  return useQuery({
    queryKey: cartKeys.active(),
    queryFn: getCart,
    staleTime: 30_000,
    enabled: enabled && hasSession,
  })
}

export function useCartCount(): number {
  const { data } = useCart()
  return data?.itemCount ?? 0
}
