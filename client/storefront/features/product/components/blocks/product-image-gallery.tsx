"use client"

import { useState } from "react"
import Image from "next/image"
import { cn } from "@/lib/utils"
import { resolveAssetUrl } from "@/lib/asset-url"
import { messages } from "@/lib/messages.ar"
import type { ProductImageInfo } from "../../types"

interface ProductImageGalleryProps {
  images: ProductImageInfo[]
  productName: string
}

export function ProductImageGallery({ images, productName }: ProductImageGalleryProps) {
  const sorted = [...images].sort((a, b) => {
    if (a.isPrimary !== b.isPrimary) return a.isPrimary ? -1 : 1
    return a.sortOrder - b.sortOrder
  })
  const [activeIndex, setActiveIndex] = useState(0)
  const active = sorted[activeIndex]
  const activeImageUrl = resolveAssetUrl(active?.imageUrl)

  if (sorted.length === 0 || !activeImageUrl) {
    return (
      <div className="bg-muted flex aspect-square items-center justify-center rounded-lg">
        <span className="text-muted-foreground text-sm">
          {messages.product.detail.noImage}
        </span>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-3">
      <div className="bg-muted relative aspect-square overflow-hidden rounded-lg">
        <Image
          src={activeImageUrl}
          alt={active.altText ?? productName}
          fill
          className="object-cover"
          sizes="(max-width: 768px) 100vw, 50vw"
          priority
        />
      </div>
      {sorted.length > 1 && (
        <div className="flex gap-2 overflow-x-auto">
          {sorted.map((img, i) => {
            const thumbUrl = resolveAssetUrl(img.imageUrl)
            if (!thumbUrl) return null

            return (
            <button
              key={img.id}
              type="button"
              onClick={() => setActiveIndex(i)}
              className={cn(
                "relative size-16 shrink-0 overflow-hidden rounded-md border-2",
                i === activeIndex ? "border-primary" : "border-transparent",
              )}
            >
              <Image
                src={thumbUrl}
                alt={img.altText ?? productName}
                fill
                className="object-cover"
                sizes="64px"
              />
            </button>
            )
          })}
        </div>
      )}
    </div>
  )
}
