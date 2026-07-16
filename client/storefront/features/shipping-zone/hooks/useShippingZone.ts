"use client"

import { useQuery } from "@tanstack/react-query"
import { getActiveShippingZones } from "../api/shipping-zone.api"
import { SHIPPING_ZONE_QUERY_KEYS } from "../constants"

export const shippingZoneKeys = {
  all: SHIPPING_ZONE_QUERY_KEYS.ALL,
  active: () => [...shippingZoneKeys.all, "active"] as const,
}

export function useActiveShippingZones(enabled = true) {
  return useQuery({
    queryKey: shippingZoneKeys.active(),
    queryFn: getActiveShippingZones,
    staleTime: 5 * 60_000,
    enabled,
  })
}
