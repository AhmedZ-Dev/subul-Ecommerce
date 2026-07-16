'use client';

import { useQuery } from '@tanstack/react-query';
import { getOrders, getOrderById } from '../api/order.api';
import type { OrderDto, OrderQueryParams } from '../types';
import { ORDER_QUERY_KEYS } from '../constants';

export const orderKeys = {
  all: ORDER_QUERY_KEYS.ALL,
  lists: () => [...orderKeys.all, 'list'] as const,
  list: (params: OrderQueryParams) => [...orderKeys.lists(), params] as const,
  details: () => [...orderKeys.all, 'detail'] as const,
  detail: (id: number) => [...orderKeys.details(), id] as const,
};

export function useOrders(params: OrderQueryParams = {}, enabled = true) {
  return useQuery({
    queryKey: orderKeys.list(params),
    queryFn: () => getOrders(params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled,
  });
}

export function useOrder(
  id: number,
  options?: { enabled?: boolean; initialData?: OrderDto },
) {
  return useQuery({
    queryKey: orderKeys.detail(id),
    queryFn: () => getOrderById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
