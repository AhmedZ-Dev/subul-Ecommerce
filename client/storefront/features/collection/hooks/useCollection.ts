"use client"

import { useQuery } from "@tanstack/react-query"
import { getActiveCollections, getCollectionById, getCollectionBySlug } from "../api/collection.api"
import { COLLECTION_QUERY_KEYS } from "../constants"
import type { CollectionDto } from "../types"

export const collectionKeys = {
  all: COLLECTION_QUERY_KEYS.ALL,
  lists: () => [...collectionKeys.all, "list"] as const,
  active: () => [...collectionKeys.lists(), "active"] as const,
  details: () => [...collectionKeys.all, "detail"] as const,
  detail: (id: number) => [...collectionKeys.details(), id] as const,
  detailBySlug: (slug: string) => [...collectionKeys.details(), "slug", slug] as const,
}

export function useActiveCollections(enabled = true) {
  return useQuery({
    queryKey: collectionKeys.active(),
    queryFn: getActiveCollections,
    staleTime: 60_000,
    enabled,
  })
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
  })
}

export function useCollectionBySlug(
  slug: string,
  options?: { enabled?: boolean; initialData?: CollectionDto },
) {
  const normalized = slug.trim().toLowerCase()
  return useQuery({
    queryKey: collectionKeys.detailBySlug(normalized),
    queryFn: () => getCollectionBySlug(normalized),
    enabled: (options?.enabled ?? true) && normalized.length > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  })
}
