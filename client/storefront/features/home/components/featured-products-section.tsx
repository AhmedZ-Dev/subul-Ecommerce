import { ProductCard } from "@/features/product"
import { messages } from "@/lib/messages.ar"
import type { StorefrontProductListItem } from "@/features/product"
import { SectionHeader } from "./section-header"

interface FeaturedProductsSectionProps {
  products: StorefrontProductListItem[]
}

export function FeaturedProductsSection({ products }: FeaturedProductsSectionProps) {
  if (products.length === 0) return null

  return (
    <section className="mb-14 md:mb-16" aria-label={messages.storefront.featuredProducts}>
      <SectionHeader
        title={messages.storefront.featuredProducts}
        description={messages.storefront.featuredProductsDescription}
        viewAllHref="/products"
      />

      <div className="grid grid-cols-2 gap-3 sm:gap-4 md:grid-cols-3 lg:grid-cols-4">
        {products.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </section>
  )
}
