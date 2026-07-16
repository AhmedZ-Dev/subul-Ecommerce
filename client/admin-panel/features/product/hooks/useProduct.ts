'use client';

import { useQuery } from '@tanstack/react-query';
import { getProducts, getProductById } from '../api/product.api';
import type { ProductDto, ProductQueryParams } from '../types';
import { PRODUCT_QUERY_KEYS } from '../constants';

export const productKeys = {
  all: PRODUCT_QUERY_KEYS.ALL,
  lists: () => [...productKeys.all, 'list'] as const,
  list: (params: ProductQueryParams) => [...productKeys.lists(), params] as const,
  details: () => [...productKeys.all, 'detail'] as const,
  detail: (id: number) => [...productKeys.details(), id] as const,
};

export function useProducts(params: ProductQueryParams = {}, enabled = true) {
  return useQuery({
    queryKey: productKeys.list(params),
    queryFn: () => getProducts(params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled,
  });
}

export function useProduct(
  id: number,
  options?: { enabled?: boolean; initialData?: ProductDto },
) {
  return useQuery({
    queryKey: productKeys.detail(id),
    queryFn: () => getProductById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
