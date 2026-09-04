"use client"

import { useQuery } from "@tanstack/react-query"
import { getProductFilterOptions } from "../api/product.api"
import { productKeys } from "./useProduct"

/** Facet counts follow the active search so they always describe the visible result set. */
export function useProductFilterOptions(
  categoryId?: number,
  search?: string,
  includeDescendants?: boolean,
  enabled = true,
) {
  const normalizedSearch = search?.trim() || undefined

  return useQuery({
    queryKey: productKeys.filterOptions(categoryId, normalizedSearch, includeDescendants),
    queryFn: () =>
      getProductFilterOptions(categoryId, normalizedSearch, includeDescendants),
    placeholderData: (prev) => prev,
    staleTime: 5 * 60_000,
    enabled,
  })
}
