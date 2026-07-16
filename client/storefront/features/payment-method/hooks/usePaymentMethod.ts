"use client"

import { useQuery } from "@tanstack/react-query"
import { getActivePaymentMethods } from "../api/payment-method.api"
import { PAYMENT_METHOD_QUERY_KEYS } from "../constants"

export const paymentMethodKeys = {
  all: PAYMENT_METHOD_QUERY_KEYS.ALL,
  active: () => [...paymentMethodKeys.all, "active"] as const,
}

export function useActivePaymentMethods(enabled = true) {
  return useQuery({
    queryKey: paymentMethodKeys.active(),
    queryFn: getActivePaymentMethods,
    staleTime: 5 * 60_000,
    enabled,
  })
}
