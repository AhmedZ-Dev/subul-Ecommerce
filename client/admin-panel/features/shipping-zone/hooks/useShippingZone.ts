'use client';

import { useQuery } from '@tanstack/react-query';
import { getShippingZones, getShippingZoneById } from '../api/shipping-zone.api';
import type { ShippingZoneDto, ShippingZoneQueryParams } from '../types';
import { SHIPPING_ZONE_QUERY_KEYS } from '../constants';

export const shippingZoneKeys = {
  all: SHIPPING_ZONE_QUERY_KEYS.ALL,
  lists: () => [...shippingZoneKeys.all, 'list'] as const,
  list: (params: ShippingZoneQueryParams) => [...shippingZoneKeys.lists(), params] as const,
  details: () => [...shippingZoneKeys.all, 'detail'] as const,
  detail: (id: number) => [...shippingZoneKeys.details(), id] as const,
};

export function useShippingZones(params: ShippingZoneQueryParams = {}, enabled = true) {
  return useQuery({
    queryKey: shippingZoneKeys.list(params),
    queryFn: () => getShippingZones(params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled,
  });
}

export function useShippingZone(
  id: number,
  options?: { enabled?: boolean; initialData?: ShippingZoneDto },
) {
  return useQuery({
    queryKey: shippingZoneKeys.detail(id),
    queryFn: () => getShippingZoneById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
