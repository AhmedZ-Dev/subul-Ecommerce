"use client"

import Link from "next/link"
import Image from "next/image"
import { Package } from "lucide-react"
import { PageContainer } from "@/components/layout/page-container"
import { resolveAssetUrl } from "@/lib/asset-url"
import { formatCurrency, getProductName, messages } from "@/lib/messages.ar"
import { useCollection } from "../../hooks/useCollection"
import type { CollectionDto } from "../../types"

interface CollectionPageProps {
  collectionId: number
  initialCollection?: CollectionDto
}

export function CollectionPage({ collectionId, initialCollection }: CollectionPageProps) {
  const { data: collection } = useCollection(collectionId, {
    initialData: initialCollection,
  })

  if (!collection) {
    return (
      <PageContainer>
        <p className="py-16 text-center">{messages.collection.empty}</p>
      </PageContainer>
    )
  }

  const name = getProductName(collection.nameAr, collection.nameEn)
  const description = collection.descriptionAr ?? collection.descriptionEn

  return (
    <PageContainer>
      <div className="mb-8">
        <h1 className="text-2xl font-bold tracking-tight md:text-3xl">{name}</h1>
        {description && (
          <p className="text-muted-foreground mt-2 max-w-2xl text-sm leading-relaxed md:text-base">
            {description}
          </p>
        )}
      </div>

      {collection.products.length === 0 ? (
        <p className="py-16 text-center">{messages.collection.empty}</p>
      ) : (
        <div className="grid grid-cols-2 gap-3 sm:gap-4 md:grid-cols-3 lg:grid-cols-4">
          {collection.products.map((product) => {
            const productName = getProductName(product.nameAr, product.nameEn)
            const imageUrl = resolveAssetUrl(product.primaryImageUrl)

            return (
              <Link
                key={product.productId}
                href={`/products/${product.slug}`}
                className="group overflow-hidden rounded-2xl bg-card ring-1 ring-foreground/8 transition-[transform,box-shadow,ring-color] duration-300 hover:-translate-y-0.5 hover:shadow-lg hover:ring-primary/25"
              >
                <div className="relative aspect-square overflow-hidden bg-muted">
                  {imageUrl ? (
                    <Image
                      src={imageUrl}
                      alt={productName}
                      fill
                      className="object-cover transition-transform duration-500 group-hover:scale-105"
                      sizes="(max-width: 640px) 50vw, (max-width: 1024px) 33vw, 25vw"
                    />
                  ) : (
                    <div className="text-muted-foreground absolute inset-0 flex flex-col items-center justify-center gap-2">
                      <Package className="size-10 opacity-40" aria-hidden />
                      <span className="text-xs">{messages.product.detail.noImage}</span>
                    </div>
                  )}
                </div>
                <div className="space-y-1 p-3 sm:p-4">
                  <p className="line-clamp-2 text-sm font-semibold leading-snug">
                    {productName}
                  </p>
                  <p className="text-primary text-base font-bold tabular-nums">
                    {formatCurrency(product.price, product.currency)}
                  </p>
                </div>
              </Link>
            )
          })}
        </div>
      )}
    </PageContainer>
  )
}
