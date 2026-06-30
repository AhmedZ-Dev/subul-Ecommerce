'use client';

import { useQuery } from '@tanstack/react-query';
import { getCollections, getCollectionById } from '../api/collection.api';
import type { CollectionDto, CollectionQueryParams } from '../types';
import { COLLECTION_QUERY_KEYS } from '../constants';

export const collectionKeys = {
  all: COLLECTION_QUERY_KEYS.ALL,
  lists: () => [...collectionKeys.all, 'list'] as const,
  list: (params: CollectionQueryParams) => [...collectionKeys.lists(), params] as const,
  details: () => [...collectionKeys.all, 'detail'] as const,
  detail: (id: number) => [...collectionKeys.details(), id] as const,
};

export function useCollections(params: CollectionQueryParams = {}, enabled = true) {
  return useQuery({
    queryKey: collectionKeys.list(params),
    queryFn: () => getCollections(params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled,
  });
}

export function useCollection(
  id: number,
  options?: { enabled?: boolean; initialData?: CollectionDto },
) {
  return useQuery({
    queryKey: collectionKeys.detail(id),
    queryFn: () => getCollectionById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
