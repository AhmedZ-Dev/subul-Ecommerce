"use client"

import { useQuery } from "@tanstack/react-query"
import {
  getStorefrontProductById,
  getStorefrontProductBySlug,
  getStorefrontProductImages,
  getStorefrontProducts,
} from "../api/product.api"
import { PRODUCT_QUERY_KEYS } from "../constants"
import type { ProductQueryParams, StorefrontProductDetail } from "../types"

export const productKeys = {
  all: PRODUCT_QUERY_KEYS.ALL,
  lists: () => [...productKeys.all, "list"] as const,
  list: (params: ProductQueryParams) => [...productKeys.lists(), params] as const,
  filterOptions: (categoryId?: number) =>
    [...productKeys.all, "filter-options", categoryId ?? "all"] as const,
  details: () => [...productKeys.all, "detail"] as const,
  detail: (id: number) => [...productKeys.details(), id] as const,
  detailBySlug: (slug: string) => [...productKeys.details(), "slug", slug] as const,
  images: (id: number) => [...productKeys.details(), id, "images"] as const,
}

export function useStorefrontProducts(params: ProductQueryParams = {}, enabled = true) {
  return useQuery({
    queryKey: productKeys.list(params),
    queryFn: () => getStorefrontProducts(params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled,
  })
}

export function useStorefrontProduct(
  id: number,
  options?: { enabled?: boolean; initialData?: StorefrontProductDetail },
) {
  return useQuery({
    queryKey: productKeys.detail(id),
    queryFn: () => getStorefrontProductById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  })
}

export function useStorefrontProductBySlug(
  slug: string,
  options?: { enabled?: boolean; initialData?: StorefrontProductDetail },
) {
  const normalized = slug.trim().toLowerCase()
  return useQuery({
    queryKey: productKeys.detailBySlug(normalized),
    queryFn: () => getStorefrontProductBySlug(normalized),
    enabled: (options?.enabled ?? true) && normalized.length > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  })
}

export function useStorefrontProductImages(productId: number, enabled = true) {
  return useQuery({
    queryKey: productKeys.images(productId),
    queryFn: () => getStorefrontProductImages(productId),
    enabled: enabled && productId > 0,
    staleTime: 60_000,
  })
}
