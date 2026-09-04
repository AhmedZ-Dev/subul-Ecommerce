import type { Metadata } from "next"
import { Suspense } from "react"
import { ProductListingPage } from "@/features/product"
import { messages } from "@/lib/messages.ar"
import { buildSocialMetadata } from "@/lib/open-graph"

export const metadata: Metadata = {
  title: messages.product.listing.title,
  description: messages.product.listing.description,
  ...buildSocialMetadata({
    title: messages.product.listing.title,
    description: messages.product.listing.description,
    path: "/products",
  }),
}

export default function ProductsPage() {
  return (
    <Suspense fallback={<div className="p-8 text-center">{messages.common.loading}</div>}>
      <ProductListingPage />
    </Suspense>
  )
}
