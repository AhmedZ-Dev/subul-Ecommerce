"use client"

import { X } from "lucide-react"
import { useQueryStates } from "nuqs"
import { Badge } from "@/components/ui/badge"
import { useProductFilterOptions } from "../../hooks/useProductFilterOptions"
import { productListingParsers } from "../../search-params"
import { getCategoryName, messages } from "@/lib/messages.ar"

interface ProductActiveFilterChipsProps {
  categoryId?: number
}

export function ProductActiveFilterChips({ categoryId }: ProductActiveFilterChipsProps) {
  const [params, setParams] = useQueryStates(productListingParsers, {
    history: "push",
    shallow: true,
  })
  const { data: filterOptions } = useProductFilterOptions(categoryId)

  const chips: Array<{ key: string; label: string; onRemove: () => void }> = []

  params.brandIds.forEach((brandId) => {
    const brand = filterOptions?.brands.find((item) => item.id === brandId)
    chips.push({
      key: `brand-${brandId}`,
      label: brand?.name ?? String(brandId),
      onRemove: () =>
        setParams({
          brandIds: params.brandIds.filter((id) => id !== brandId),
          page: 1,
        }),
    })
  })

  if (params.minPrice != null) {
    chips.push({
      key: "min-price",
      label: `${messages.product.filters.priceMin}: ${params.minPrice}`,
      onRemove: () => setParams({ minPrice: null, page: 1 }),
    })
  }

  if (params.maxPrice != null) {
    chips.push({
      key: "max-price",
      label: `${messages.product.filters.priceMax}: ${params.maxPrice}`,
      onRemove: () => setParams({ maxPrice: null, page: 1 }),
    })
  }

  if (params.inStock === true) {
    chips.push({
      key: "in-stock",
      label: messages.product.filters.inStockOnly,
      onRemove: () => setParams({ inStock: null, page: 1 }),
    })
  }

  Object.entries(params.attrs).forEach(([groupId, values]) => {
    const group = filterOptions?.attributeGroups.find((item) => String(item.id) === groupId)
    const groupName = group ? getCategoryName(group.nameAr, group.nameEn) : groupId
    values.forEach((value) => {
      chips.push({
        key: `attr-${groupId}-${value}`,
        label: `${groupName}: ${value}`,
        onRemove: () => {
          const nextValues = values.filter((item) => item !== value)
          const nextAttrs = { ...params.attrs }
          if (nextValues.length === 0) {
            delete nextAttrs[groupId]
          } else {
            nextAttrs[groupId] = nextValues
          }
          setParams({ attrs: nextAttrs, page: 1 })
        },
      })
    })
  })

  if (chips.length === 0) return null

  return (
    <div className="mb-4 flex flex-wrap gap-2">
      {chips.map((chip) => (
        <Badge key={chip.key} variant="secondary" className="gap-1 pe-1">
          <span>{chip.label}</span>
          <button
            type="button"
            className="hover:bg-muted rounded-full p-0.5"
            onClick={chip.onRemove}
            aria-label={messages.product.filters.clearAll}
          >
            <X className="size-3" />
          </button>
        </Badge>
      ))}
    </div>
  )
}
