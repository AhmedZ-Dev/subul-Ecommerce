"use client"

import { PageContainer } from "@/components/layout/page-container"
import { ProductGrid, type StorefrontProductListItem } from "@/features/product"
import { getProductName, messages } from "@/lib/messages.ar"
import { useCollection } from "../../hooks/useCollection"
import type { CollectionDto, CollectionProduct } from "../../types"

interface CollectionPageProps {
  collectionId: number
  initialCollection?: CollectionDto
}

/** Collection entries render through the catalog card so badges and the CTA match the rest of the store. */
function toListItem(product: CollectionProduct): StorefrontProductListItem {
  return {
    id: product.productId,
    nameEn: product.nameEn,
    nameAr: product.nameAr,
    slug: product.slug,
    price: product.price,
    compareAtPrice: product.compareAtPrice,
    currency: product.currency,
    stockQuantity: product.stockQuantity,
    isFeatured: product.isFeatured,
    category: null,
    brand: product.brand,
    primaryImageUrl: product.primaryImageUrl,
  }
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
        <ProductGrid products={collection.products.map(toListItem)} />
      )}
    </PageContainer>
  )
}
