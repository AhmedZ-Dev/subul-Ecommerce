import { ProductCard } from "@/features/product"
import type { StorefrontProductListItem } from "@/features/product"
import { SectionHeader } from "./section-header"

interface CategoryProductsRowProps {
  title: string
  viewAllHref: string
  products: StorefrontProductListItem[]
}

export function CategoryProductsRow({
  title,
  viewAllHref,
  products,
}: CategoryProductsRowProps) {
  if (products.length === 0) return null

  return (
    <section className="mb-14 md:mb-16">
      <SectionHeader title={title} viewAllHref={viewAllHref} />

      <div className="grid grid-cols-2 gap-3 sm:gap-4 md:grid-cols-3 lg:grid-cols-4">
        {products.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </section>
  )
}
