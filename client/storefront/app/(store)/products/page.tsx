import type { Metadata } from "next"
import { Suspense } from "react"
import { ProductListingPage } from "@/features/product"
import { messages } from "@/lib/messages.ar"

export const metadata: Metadata = {
  title: messages.product.listing.title,
  description: messages.product.listing.description,
}

export default function ProductsPage() {
  return (
    <Suspense fallback={<div className="p-8 text-center">{messages.common.loading}</div>}>
      <ProductListingPage />
    </Suspense>
  )
}
