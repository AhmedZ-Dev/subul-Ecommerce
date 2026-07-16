"use client"

import { useQuery } from "@tanstack/react-query"
import { getProductFilterOptions } from "../api/product.api"
import { productKeys } from "./useProduct"

export function useProductFilterOptions(categoryId?: number, enabled = true) {
  return useQuery({
    queryKey: productKeys.filterOptions(categoryId),
    queryFn: () => getProductFilterOptions(categoryId),
    staleTime: 5 * 60_000,
    enabled,
  })
}
