"use client"

import Link from "next/link"
import Image from "next/image"
import { Package } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { resolveAssetUrl } from "@/lib/asset-url"
import { AddToCartButton } from "./add-to-cart-button"
import { formatCurrency, getProductName, messages } from "@/lib/messages.ar"
import { cn } from "@/lib/utils"
import type { StorefrontProductListItem } from "../../types"

interface ProductCardProps {
  product: StorefrontProductListItem
}

function getDiscountPercent(price: number, compareAtPrice: number | null): number | null {
  if (compareAtPrice == null || compareAtPrice <= price) return null
  return Math.round(((compareAtPrice - price) / compareAtPrice) * 100)
}

export function ProductCard({ product }: ProductCardProps) {
  const name = getProductName(product.nameAr, product.nameEn)
  const outOfStock = product.stockQuantity <= 0
  const discount = getDiscountPercent(product.price, product.compareAtPrice)
  const imageUrl = resolveAssetUrl(product.primaryImageUrl)
  const hasComparePrice =
    product.compareAtPrice != null && product.compareAtPrice > product.price

  return (
    <article
      className={cn(
        "product-card-hover group/card flex h-full flex-col overflow-hidden rounded-2xl bg-card ring-1 ring-foreground/8 transition-[box-shadow,transform,ring-color]",
        "hover:ring-primary/20",
        outOfStock && "opacity-95",
      )}
    >
      <Link
        href={`/products/${product.slug}`}
        className="relative block overflow-hidden"
        aria-label={name}
      >
        <div className="relative aspect-4/5 overflow-hidden bg-linear-to-b from-muted/30 via-muted/50 to-muted sm:aspect-square">
          {imageUrl ? (
            <Image
              src={imageUrl}
              alt={name}
              fill
              className="object-cover transition-transform duration-500 ease-out group-hover/card:scale-[1.04]"
              sizes="(max-width: 640px) 50vw, (max-width: 1024px) 33vw, 25vw"
            />
          ) : (
            <>
              <div className="hero-gradient absolute inset-0 opacity-15" aria-hidden />
              <div className="text-muted-foreground absolute inset-0 flex flex-col items-center justify-center gap-2">
                <Package className="size-10 opacity-40" aria-hidden />
                <span className="text-xs">{messages.product.detail.noImage}</span>
              </div>
            </>
          )}

          <div className="pointer-events-none absolute inset-x-0 top-0 flex items-start justify-between gap-2 p-2.5">
            {product.isFeatured ? (
              <Badge className="border-0 bg-primary/95 text-primary-foreground shadow-sm backdrop-blur-sm">
                {messages.product.featured}
              </Badge>
            ) : (
              <span aria-hidden />
            )}
            {discount != null && (
              <Badge
                className="border-0 font-semibold text-white shadow-sm"
                style={{ backgroundColor: "var(--price-badge-bg)" }}
              >
                -{discount}%
              </Badge>
            )}
          </div>

          {outOfStock && (
            <div className="absolute inset-0 flex items-center justify-center bg-background/55 backdrop-blur-[1px]">
              <Badge variant="destructive" className="px-3 py-1 text-xs shadow-sm">
                {messages.product.outOfStock}
              </Badge>
            </div>
          )}
        </div>
      </Link>

      <div className="flex flex-1 flex-col gap-1.5 p-3 sm:gap-2 sm:p-4">
        {product.brand && (
          <p className="text-muted-foreground truncate text-[11px] font-medium tracking-wide uppercase sm:text-xs">
            {product.brand.name}
          </p>
        )}

        <Link
          href={`/products/${product.slug}`}
          className="hover:text-primary transition-colors"
        >
          <h3 className="line-clamp-2 min-h-10 text-sm leading-snug font-semibold sm:min-h-0 sm:text-[0.9375rem]">
            {name}
          </h3>
        </Link>

        <div className="mt-auto flex flex-wrap items-baseline gap-x-2 gap-y-0.5 pt-1">
          <span className="text-primary text-base font-bold tabular-nums sm:text-lg">
            {formatCurrency(product.price, product.currency)}
          </span>
          {hasComparePrice && (
            <span className="text-muted-foreground text-xs line-through tabular-nums sm:text-sm">
              {formatCurrency(product.compareAtPrice!, product.currency)}
            </span>
          )}
        </div>

        <AddToCartButton
          productId={product.id}
          disabled={outOfStock}
          showIcon
          className="mt-2 h-10 w-full rounded-lg text-sm font-medium shadow-none sm:h-9"
        />
      </div>
    </article>
  )
}
