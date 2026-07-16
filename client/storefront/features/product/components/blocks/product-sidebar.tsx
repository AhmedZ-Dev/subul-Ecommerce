"use client"

import { useCallback, useEffect, useState } from "react"
import { useQueryStates } from "nuqs"
import { Minus, Plus } from "lucide-react"
import { Checkbox } from "@/components/ui/checkbox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { Skeleton } from "@/components/ui/skeleton"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { useStorefrontCategories } from "@/features/category"
import { getCategoryName, messages } from "@/lib/messages.ar"
import { cn } from "@/lib/utils"
import { useProductFilterOptions } from "../../hooks/useProductFilterOptions"
import { productListingParsers } from "../../search-params"

interface ProductSidebarProps {
  categoryId?: number
  className?: string
  /** Use inside mobile Sheet — hides duplicate title and adjusts layout */
  variant?: "default" | "sheet"
}

function FilterCheckboxRow({
  checked,
  onCheckedChange,
  label,
  count,
}: {
  checked: boolean
  onCheckedChange: (checked: boolean) => void
  label: string
  count?: number
}) {
  return (
    <label className="flex min-h-10 cursor-pointer items-center gap-2.5 rounded-md py-1.5 text-sm active:bg-muted/50">
      <Checkbox
        className="shrink-0"
        checked={checked}
        onCheckedChange={(value) => onCheckedChange(value === true)}
      />
      <span className="flex min-w-0 flex-1 items-center gap-1.5">
        <span className="truncate">{label}</span>
        {count != null && (
          <span className="text-muted-foreground shrink-0 text-xs tabular-nums">
            ({count})
          </span>
        )}
      </span>
    </label>
  )
}

function FilterSection({
  title,
  defaultOpen = true,
  children,
}: {
  title: string
  defaultOpen?: boolean
  children: React.ReactNode
}) {
  const [open, setOpen] = useState(defaultOpen)

  return (
    <div className="border-b border-border pb-4 last:border-b-0">
      <button
        type="button"
        className="flex min-h-11 w-full items-center justify-between gap-3 py-2 text-start"
        onClick={() => setOpen((prev) => !prev)}
      >
        <span className="text-sm font-semibold">{title}</span>
        {open ? (
          <Minus className="text-muted-foreground size-4 shrink-0" />
        ) : (
          <Plus className="text-muted-foreground size-4 shrink-0" />
        )}
      </button>
      {open && <div className="space-y-0.5 pt-1">{children}</div>}
    </div>
  )
}

export function ProductSidebar({
  categoryId,
  className,
  variant = "default",
}: ProductSidebarProps) {
  const isSheet = variant === "sheet"
  const [params, setParams] = useQueryStates(productListingParsers, {
    history: "push",
    shallow: true,
  })
  const { data: filterOptions, isLoading } = useProductFilterOptions(categoryId)
  const { data: categoriesData } = useStorefrontCategories(
    { limit: 100, isActive: true },
    categoryId == null,
  )

  const [minPriceInput, setMinPriceInput] = useState(
    params.minPrice != null ? String(params.minPrice) : "",
  )
  const [maxPriceInput, setMaxPriceInput] = useState(
    params.maxPrice != null ? String(params.maxPrice) : "",
  )

  useEffect(() => {
    setMinPriceInput(params.minPrice != null ? String(params.minPrice) : "")
    setMaxPriceInput(params.maxPrice != null ? String(params.maxPrice) : "")
  }, [params.minPrice, params.maxPrice])

  useEffect(() => {
    const timer = window.setTimeout(() => {
      const min = minPriceInput.trim() ? Number(minPriceInput) : null
      const max = maxPriceInput.trim() ? Number(maxPriceInput) : null
      if (
        (minPriceInput.trim() && Number.isNaN(min)) ||
        (maxPriceInput.trim() && Number.isNaN(max))
      ) {
        return
      }
      if (min === params.minPrice && max === params.maxPrice) return
      setParams({ minPrice: min, maxPrice: max, page: 1 })
    }, 400)

    return () => window.clearTimeout(timer)
  }, [minPriceInput, maxPriceInput, params.minPrice, params.maxPrice, setParams])

  const toggleBrand = useCallback(
    (brandId: number, checked: boolean) => {
      const next = checked
        ? [...params.brandIds, brandId]
        : params.brandIds.filter((id) => id !== brandId)
      setParams({ brandIds: next, brandId: null, page: 1 })
    },
    [params.brandIds, setParams],
  )

  const toggleAttributeValue = useCallback(
    (groupId: number, value: string, checked: boolean) => {
      const key = String(groupId)
      const current = params.attrs[key] ?? []
      const nextValues = checked
        ? [...current, value]
        : current.filter((item) => item !== value)
      const nextAttrs = { ...params.attrs }
      if (nextValues.length === 0) {
        delete nextAttrs[key]
      } else {
        nextAttrs[key] = nextValues
      }
      setParams({ attrs: nextAttrs, page: 1 })
    },
    [params.attrs, setParams],
  )

  const clearAllFilters = () => {
    setMinPriceInput("")
    setMaxPriceInput("")
    setParams({
      brandIds: [],
      brandId: null,
      minPrice: null,
      maxPrice: null,
      inStock: null,
      attrs: {},
      categoryId: categoryId ?? null,
      page: 1,
    })
  }

  if (isLoading) {
    return (
      <div className={cn("space-y-4", className)}>
        <Skeleton className="h-6 w-24" />
        <Skeleton className="h-32 w-full" />
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-40 w-full" />
      </div>
    )
  }

  return (
    <div className={cn(isSheet ? "space-y-3" : "space-y-4", className)}>
      {isSheet ? (
        <div className="flex justify-end pb-1">
          <button
            type="button"
            className="text-primary min-h-10 px-1 text-xs font-medium hover:underline"
            onClick={clearAllFilters}
          >
            {messages.product.filters.clearAll}
          </button>
        </div>
      ) : (
        <>
          <div className="flex items-center justify-between">
            <h2 className="text-base font-semibold">{messages.product.filters.title}</h2>
            <button
              type="button"
              className="text-primary text-xs font-medium hover:underline"
              onClick={clearAllFilters}
            >
              {messages.product.filters.clearAll}
            </button>
          </div>
          <Separator />
        </>
      )}

      {categoryId == null && (
        <FilterSection title={messages.product.filters.category}>
          <Select
            value={params.categoryId?.toString() ?? "all"}
            onValueChange={(value) =>
              setParams({
                categoryId: value === "all" ? null : Number(value),
                page: 1,
              })
            }
          >
            <SelectTrigger className="h-10">
              <SelectValue placeholder={messages.product.filters.allCategories} />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">{messages.product.filters.allCategories}</SelectItem>
              {categoriesData?.items.map((cat) => (
                <SelectItem key={cat.id} value={cat.id.toString()}>
                  {getCategoryName(cat.nameAr, cat.nameEn)}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FilterSection>
      )}

      {filterOptions?.brands && filterOptions.brands.length > 0 && (
        <FilterSection title={messages.product.filters.brand}>
          {filterOptions.brands.map((brand) => (
            <FilterCheckboxRow
              key={brand.id}
              checked={params.brandIds.includes(brand.id)}
              onCheckedChange={(checked) => toggleBrand(brand.id, checked)}
              label={brand.name}
              count={brand.count}
            />
          ))}
        </FilterSection>
      )}

      <FilterSection title={messages.product.filters.priceRange}>
        <div
          className={cn(
            "grid gap-3",
            isSheet ? "grid-cols-1" : "grid-cols-2 gap-2",
          )}
        >
          <div>
            <Label className="text-muted-foreground text-xs">
              {messages.product.filters.priceMin}
            </Label>
            <Input
              type="number"
              min={0}
              inputMode="numeric"
              dir="ltr"
              placeholder={String(filterOptions?.priceRange.min ?? 0)}
              value={minPriceInput}
              onChange={(e) => setMinPriceInput(e.target.value)}
              className="h-10"
            />
          </div>
          <div>
            <Label className="text-muted-foreground text-xs">
              {messages.product.filters.priceMax}
            </Label>
            <Input
              type="number"
              min={0}
              inputMode="numeric"
              dir="ltr"
              placeholder={String(filterOptions?.priceRange.max ?? 0)}
              value={maxPriceInput}
              onChange={(e) => setMaxPriceInput(e.target.value)}
              className="h-10"
            />
          </div>
        </div>
      </FilterSection>

      <FilterSection title={messages.product.filters.inStockOnly} defaultOpen={false}>
        <FilterCheckboxRow
          checked={params.inStock === true}
          onCheckedChange={(checked) =>
            setParams({ inStock: checked ? true : null, page: 1 })
          }
          label={messages.product.inStock}
        />
      </FilterSection>

      {filterOptions?.attributeGroups.map((group) => (
        <FilterSection
          key={group.id}
          title={getCategoryName(group.nameAr, group.nameEn)}
          defaultOpen={false}
        >
          {group.values.map((facet) => {
            const selected = params.attrs[String(group.id)]?.includes(facet.value) ?? false
            return (
              <FilterCheckboxRow
                key={`${group.id}-${facet.value}`}
                checked={selected}
                onCheckedChange={(checked) =>
                  toggleAttributeValue(group.id, facet.value, checked)
                }
                label={facet.value}
                count={facet.count}
              />
            )
          })}
        </FilterSection>
      ))}

      {filterOptions &&
        filterOptions.brands.length === 0 &&
        filterOptions.attributeGroups.length === 0 && (
          <p className="text-muted-foreground text-sm">{messages.product.filters.noFilters}</p>
        )}
    </div>
  )
}
