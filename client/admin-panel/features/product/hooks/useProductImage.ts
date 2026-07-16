'use client';

import { useQuery } from '@tanstack/react-query';
import { getProductImages, getProductImageById } from '../api/product-image.api';
import type { ProductImageInfo, ProductImageQueryParams } from '../types';
import { PRODUCT_IMAGE_QUERY_KEYS } from '../constants';

export const productImageKeys = {
  all: PRODUCT_IMAGE_QUERY_KEYS.ALL,
  lists: () => [...productImageKeys.all, 'list'] as const,
  list: (productId: number, params: ProductImageQueryParams) =>
    [...productImageKeys.lists(), productId, params] as const,
  details: () => [...productImageKeys.all, 'detail'] as const,
  detail: (productId: number, id: number) =>
    [...productImageKeys.details(), productId, id] as const,
};

export function useProductImages(
  productId: number,
  params: ProductImageQueryParams = {},
  enabled = true,
) {
  return useQuery({
    queryKey: productImageKeys.list(productId, params),
    queryFn: () => getProductImages(productId, params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled: enabled && productId > 0,
  });
}

export function useProductImage(
  productId: number,
  imageId: number,
  options?: { enabled?: boolean; initialData?: ProductImageInfo },
) {
  return useQuery({
    queryKey: productImageKeys.detail(productId, imageId),
    queryFn: () => getProductImageById(productId, imageId),
    enabled: (options?.enabled ?? true) && productId > 0 && imageId > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
