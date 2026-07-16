'use client';

import { useQuery } from '@tanstack/react-query';
import { getCollectionProducts, getCollectionProductById } from '../api/collection-product.api';
import type { CollectionProductInfo, CollectionProductQueryParams } from '../types';
import { COLLECTION_PRODUCT_QUERY_KEYS } from '../constants';

export const collectionProductKeys = {
  all: COLLECTION_PRODUCT_QUERY_KEYS.ALL,
  lists: () => [...collectionProductKeys.all, 'list'] as const,
  list: (collectionId: number, params: CollectionProductQueryParams) =>
    [...collectionProductKeys.lists(), collectionId, params] as const,
  details: () => [...collectionProductKeys.all, 'detail'] as const,
  detail: (collectionId: number, id: number) =>
    [...collectionProductKeys.details(), collectionId, id] as const,
};

export function useCollectionProducts(
  collectionId: number,
  params: CollectionProductQueryParams = {},
  enabled = true,
) {
  return useQuery({
    queryKey: collectionProductKeys.list(collectionId, params),
    queryFn: () => getCollectionProducts(collectionId, params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled: enabled && collectionId > 0,
  });
}

export function useCollectionProduct(
  collectionId: number,
  id: number,
  options?: { enabled?: boolean; initialData?: CollectionProductInfo },
) {
  return useQuery({
    queryKey: collectionProductKeys.detail(collectionId, id),
    queryFn: () => getCollectionProductById(collectionId, id),
    enabled: (options?.enabled ?? true) && collectionId > 0 && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
