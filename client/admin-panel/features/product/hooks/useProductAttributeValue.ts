'use client';

import { useQuery } from '@tanstack/react-query';
import {
  getProductAttributeValues,
  getProductAttributeValueById,
} from '../api/product-attribute-value.api';
import type { ProductAttributeValueInfo, ProductAttributeValueQueryParams } from '../types';
import { PRODUCT_ATTR_VALUE_QUERY_KEYS } from '../constants';

export const productAttributeValueKeys = {
  all: PRODUCT_ATTR_VALUE_QUERY_KEYS.ALL,
  lists: () => [...productAttributeValueKeys.all, 'list'] as const,
  list: (productId: number, params: ProductAttributeValueQueryParams) =>
    [...productAttributeValueKeys.lists(), productId, params] as const,
  details: () => [...productAttributeValueKeys.all, 'detail'] as const,
  detail: (productId: number, id: number) =>
    [...productAttributeValueKeys.details(), productId, id] as const,
};

export function useProductAttributeValues(
  productId: number,
  params: ProductAttributeValueQueryParams = {},
  enabled = true,
) {
  return useQuery({
    queryKey: productAttributeValueKeys.list(productId, params),
    queryFn: () => getProductAttributeValues(productId, params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled: enabled && productId > 0,
  });
}

export function useProductAttributeValue(
  productId: number,
  valueId: number,
  options?: { enabled?: boolean; initialData?: ProductAttributeValueInfo },
) {
  return useQuery({
    queryKey: productAttributeValueKeys.detail(productId, valueId),
    queryFn: () => getProductAttributeValueById(productId, valueId),
    enabled: (options?.enabled ?? true) && productId > 0 && valueId > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
