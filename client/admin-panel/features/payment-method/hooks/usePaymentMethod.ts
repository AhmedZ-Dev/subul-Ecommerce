'use client';

import { useQuery } from '@tanstack/react-query';
import { getPaymentMethods, getPaymentMethodById } from '../api/payment-method.api';
import type { PaymentMethodDto, PaymentMethodQueryParams } from '../types';
import { PAYMENT_METHOD_QUERY_KEYS } from '../constants';

export const paymentMethodKeys = {
  all: PAYMENT_METHOD_QUERY_KEYS.ALL,
  lists: () => [...paymentMethodKeys.all, 'list'] as const,
  list: (params: PaymentMethodQueryParams) => [...paymentMethodKeys.lists(), params] as const,
  details: () => [...paymentMethodKeys.all, 'detail'] as const,
  detail: (id: number) => [...paymentMethodKeys.details(), id] as const,
};

export function usePaymentMethods(params: PaymentMethodQueryParams = {}, enabled = true) {
  return useQuery({
    queryKey: paymentMethodKeys.list(params),
    queryFn: () => getPaymentMethods(params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled,
  });
}

export function usePaymentMethod(
  id: number,
  options?: { enabled?: boolean; initialData?: PaymentMethodDto },
) {
  return useQuery({
    queryKey: paymentMethodKeys.detail(id),
    queryFn: () => getPaymentMethodById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
