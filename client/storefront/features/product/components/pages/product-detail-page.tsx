"use client"

import { useState } from "react"
import Link from "next/link"
import { Minus, Plus } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { PageContainer } from "@/components/layout/page-container"
import { AddToCartButton } from "../blocks/add-to-cart-button"
import { ProductImageGallery } from "../blocks/product-image-gallery"
import { ProductVariantSelector } from "../blocks/product-variant-selector"
import {
  useStorefrontProduct,
  useStorefrontProductImages,
} from "../../hooks/useProduct"
import {
  formatCurrency,
  getCategoryName,
  getProductName,
  messages,
} from "@/lib/messages.ar"
import type { StorefrontProductDetail } from "../../types"

interface ProductDetailPageProps {
  productId: number
  initialProduct?: StorefrontProductDetail
}

export function ProductDetailPage({ productId, initialProduct }: ProductDetailPageProps) {
  const { data: product } = useStorefrontProduct(productId, {
    initialData: initialProduct,
  })
  const { data: images = [] } = useStorefrontProductImages(productId)
  const [selectedVariantId, setSelectedVariantId] = useState<number | undefined>()
  const [quantity, setQuantity] = useState(1)

  if (!product) {
    return (
      <PageContainer>
        <p className="py-16 text-center">{messages.product.noProducts}</p>
      </PageContainer>
    )
  }

  const name = getProductName(product.nameAr, product.nameEn)
  const description = product.descriptionAr ?? product.descriptionEn
  const selectedVariant = product.variants.find((v) => v.id === selectedVariantId)
  const displayPrice = selectedVariant?.price ?? product.price
  const displayCompareAt = selectedVariant?.compareAtPrice ?? product.compareAtPrice
  const stock = selectedVariant?.stockQuantity ?? product.stockQuantity
  const outOfStock = stock <= 0
  const minQty = product.minOrderQuantity
  const addDisabled = outOfStock || (product.variants.length > 0 && !selectedVariantId)

  return (
    <PageContainer className="pb-28 lg:pb-6">
      <div className="grid gap-8 lg:grid-cols-2">
        <ProductImageGallery images={images} productName={name} />

        <div className="flex flex-col gap-4">
          {product.brand && (
            <p className="text-muted-foreground text-sm">{product.brand.name}</p>
          )}
          <h1 className="text-2xl font-bold md:text-3xl">{name}</h1>

          {product.category && (
            <Link
              href={`/categories/${product.category.slug}`}
              className="text-primary text-sm hover:underline"
            >
              {getCategoryName(product.category.nameAr, product.category.nameEn)}
            </Link>
          )}

          <div className="flex flex-wrap items-baseline gap-3">
            <span className="text-2xl font-bold md:text-3xl">
              {formatCurrency(displayPrice, product.currency)}
            </span>
            {displayCompareAt != null && displayCompareAt > displayPrice && (
              <span className="text-muted-foreground text-lg line-through">
                {formatCurrency(displayCompareAt, product.currency)}
              </span>
            )}
          </div>

          {outOfStock ? (
            <Badge variant="destructive">{messages.product.outOfStock}</Badge>
          ) : (
            <Badge variant="secondary">{messages.product.inStock}</Badge>
          )}

          {(product.shortDescriptionAr ?? product.shortDescriptionEn) && (
            <p className="text-muted-foreground text-sm">
              {product.shortDescriptionAr ?? product.shortDescriptionEn}
            </p>
          )}

          {product.variants.length > 0 && (
            <ProductVariantSelector
              variants={product.variants}
              selectedId={selectedVariantId}
              onSelect={setSelectedVariantId}
            />
          )}

          <div className="flex flex-col gap-2 sm:flex-row sm:flex-wrap sm:items-center sm:gap-3">
            <span className="text-sm font-medium">{messages.product.detail.quantity}</span>
            <div className="flex items-center gap-2">
              <Button
                type="button"
                variant="outline"
                size="icon"
                className="size-11"
                disabled={quantity <= minQty}
                onClick={() => setQuantity((q) => Math.max(minQty, q - 1))}
                aria-label={messages.cart.quantity}
              >
                <Minus className="size-4" />
              </Button>
              <span className="w-8 text-center font-medium">{quantity}</span>
              <Button
                type="button"
                variant="outline"
                size="icon"
                className="size-11"
                disabled={quantity >= stock}
                onClick={() => setQuantity((q) => Math.min(stock, q + 1))}
                aria-label={messages.cart.quantity}
              >
                <Plus className="size-4" />
              </Button>
            </div>
            {minQty > 1 && (
              <span className="text-muted-foreground text-xs">
                {messages.product.detail.minOrder(minQty)}
              </span>
            )}
          </div>

          <AddToCartButton
            productId={product.id}
            variantId={selectedVariantId}
            quantity={quantity}
            disabled={addDisabled}
            className="hidden h-11 w-full lg:inline-flex lg:w-auto"
          />

          {product.warrantyMonths > 0 && (
            <p className="text-muted-foreground text-sm">
              {messages.product.detail.warranty(product.warrantyMonths)}
            </p>
          )}
        </div>
      </div>

      {description && (
        <section className="mt-12">
          <h2 className="mb-4 text-xl font-semibold">{messages.product.detail.description}</h2>
          <p className="text-muted-foreground whitespace-pre-wrap">{description}</p>
        </section>
      )}

      {product.attributeValues.length > 0 && (
        <section className="mt-8">
          <h2 className="mb-4 text-xl font-semibold">{messages.product.detail.attributes}</h2>
          <dl className="grid gap-2 sm:grid-cols-2">
            {product.attributeValues.map((attr) => {
              const attrName = attr.attribute.nameAr ?? attr.attribute.nameEn
              const value =
                attr.valueText ??
                (attr.valueNumber != null
                  ? `${attr.valueNumber}${attr.attribute.unit ? ` ${attr.attribute.unit}` : ""}`
                  : attr.valueBoolean != null
                    ? attr.valueBoolean
                      ? messages.common.yes
                      : messages.common.no
                    : "—")
              return (
                <div key={attr.id} className="flex justify-between border-b py-2 text-sm">
                  <dt className="text-muted-foreground">{attrName}</dt>
                  <dd className="font-medium">{String(value)}</dd>
                </div>
              )
            })}
          </dl>
        </section>
      )}

      <div className="mobile-sticky-bar lg:hidden">
        <div className="container mx-auto flex items-center gap-3 px-4 py-3">
          <div className="min-w-0 flex-1">
            <p className="text-primary text-lg font-bold">
              {formatCurrency(displayPrice, product.currency)}
            </p>
          </div>
          <AddToCartButton
            productId={product.id}
            variantId={selectedVariantId}
            quantity={quantity}
            disabled={addDisabled}
            className="h-11 shrink-0 px-6"
          />
        </div>
      </div>
    </PageContainer>
  )
}
