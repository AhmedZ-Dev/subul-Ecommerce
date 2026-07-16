'use client';

import { useQuery } from '@tanstack/react-query';
import { getProductVariants, getProductVariantById } from '../api/product-variant.api';
import type { ProductVariantInfo, ProductVariantQueryParams } from '../types';
import { PRODUCT_VARIANT_QUERY_KEYS } from '../constants';

export const productVariantKeys = {
  all: PRODUCT_VARIANT_QUERY_KEYS.ALL,
  lists: () => [...productVariantKeys.all, 'list'] as const,
  list: (productId: number, params: ProductVariantQueryParams) =>
    [...productVariantKeys.lists(), productId, params] as const,
  details: () => [...productVariantKeys.all, 'detail'] as const,
  detail: (productId: number, id: number) =>
    [...productVariantKeys.details(), productId, id] as const,
};

export function useProductVariants(
  productId: number,
  params: ProductVariantQueryParams = {},
  enabled = true,
) {
  return useQuery({
    queryKey: productVariantKeys.list(productId, params),
    queryFn: () => getProductVariants(productId, params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled: enabled && productId > 0,
  });
}

export function useProductVariant(
  productId: number,
  variantId: number,
  options?: { enabled?: boolean; initialData?: ProductVariantInfo },
) {
  return useQuery({
    queryKey: productVariantKeys.detail(productId, variantId),
    queryFn: () => getProductVariantById(productId, variantId),
    enabled: (options?.enabled ?? true) && productId > 0 && variantId > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
