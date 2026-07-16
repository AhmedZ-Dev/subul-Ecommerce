import { Skeleton } from "@/components/ui/skeleton"
import { ProductCard } from "./product-card"
import type { StorefrontProductListItem } from "../../types"

interface ProductGridProps {
  products: StorefrontProductListItem[]
  loading?: boolean
}

export function ProductGrid({ products, loading }: ProductGridProps) {
  if (loading) {
    return (
      <div className="grid grid-cols-2 gap-3 min-[400px]:gap-4 sm:grid-cols-3 lg:grid-cols-4">
        {Array.from({ length: 8 }).map((_, i) => (
          <Skeleton key={i} className="aspect-4/5 rounded-2xl sm:aspect-square" />
        ))}
      </div>
    )
  }

  return (
    <div className="grid grid-cols-2 gap-3 min-[400px]:gap-4 sm:grid-cols-3 lg:grid-cols-4">
      {products.map((product) => (
        <ProductCard key={product.id} product={product} />
      ))}
    </div>
  )
}
