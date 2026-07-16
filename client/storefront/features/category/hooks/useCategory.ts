"use client"

import { useQuery } from "@tanstack/react-query"
import {
  getStorefrontCategories,
  getStorefrontCategoryById,
  getStorefrontCategoryBySlug,
  getTopLevelCategories,
} from "../api/category.api"
import { CATEGORY_QUERY_KEYS } from "../constants"
import { buildCategoryTree } from "../utils"
import type { CategoryDto, CategoryQueryParams } from "../types"

export const categoryKeys = {
  all: CATEGORY_QUERY_KEYS.ALL,
  lists: () => [...categoryKeys.all, "list"] as const,
  list: (params: CategoryQueryParams) => [...categoryKeys.lists(), params] as const,
  details: () => [...categoryKeys.all, "detail"] as const,
  detail: (id: number) => [...categoryKeys.details(), id] as const,
  detailBySlug: (slug: string) => [...categoryKeys.details(), "slug", slug] as const,
  nav: () => [...categoryKeys.all, "nav"] as const,
}

export function useStorefrontCategories(params: CategoryQueryParams = {}, enabled = true) {
  return useQuery({
    queryKey: categoryKeys.list(params),
    queryFn: () => getStorefrontCategories(params),
    staleTime: 60_000,
    enabled,
  })
}

export function useStorefrontCategory(
  id: number,
  options?: { enabled?: boolean; initialData?: CategoryDto },
) {
  return useQuery({
    queryKey: categoryKeys.detail(id),
    queryFn: () => getStorefrontCategoryById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  })
}

export function useStorefrontCategoryBySlug(
  slug: string,
  options?: { enabled?: boolean; initialData?: CategoryDto },
) {
  const normalized = slug.trim().toLowerCase()
  return useQuery({
    queryKey: categoryKeys.detailBySlug(normalized),
    queryFn: () => getStorefrontCategoryBySlug(normalized),
    enabled: (options?.enabled ?? true) && normalized.length > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  })
}

export function useCategoryTree(enabled = true) {
  const query = useStorefrontCategories(
    { limit: 1000, isActive: true, sortBy: "sortOrder", sortOrder: "asc" },
    enabled,
  )
  return {
    ...query,
    data: query.data ? buildCategoryTree(query.data.items) : undefined,
  }
}

export function useCategoryNav(enabled = true) {
  return useQuery({
    queryKey: categoryKeys.nav(),
    queryFn: getTopLevelCategories,
    staleTime: 60_000,
    enabled,
  })
}
