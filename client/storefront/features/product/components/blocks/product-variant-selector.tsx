"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { messages } from "@/lib/messages.ar"
import type { ProductVariantInfo } from "../../types"

interface ProductVariantSelectorProps {
  variants: ProductVariantInfo[]
  selectedId: number | undefined
  onSelect: (id: number | undefined) => void
}

export function ProductVariantSelector({
  variants,
  selectedId,
  onSelect,
}: ProductVariantSelectorProps) {
  const activeVariants = variants.filter((v) => v.isActive)

  if (activeVariants.length === 0) return null

  return (
    <div className="flex flex-col gap-2">
      <p className="text-sm font-medium">{messages.product.detail.variants}</p>
      <div className="flex flex-wrap gap-2">
        {activeVariants.map((variant) => (
          <Button
            key={variant.id}
            type="button"
            variant={selectedId === variant.id ? "default" : "outline"}
            size="sm"
            disabled={variant.stockQuantity <= 0}
            className={cn(variant.stockQuantity <= 0 && "opacity-50")}
            onClick={() => onSelect(variant.id)}
          >
            {variant.title ?? `خيار ${variant.id}`}
            {variant.stockQuantity <= 0 && ` (${messages.product.outOfStock})`}
          </Button>
        ))}
      </div>
    </div>
  )
}
