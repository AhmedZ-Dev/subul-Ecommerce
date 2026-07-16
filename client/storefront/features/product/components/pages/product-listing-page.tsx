"use client"

import { useQueryStates } from "nuqs"
import { Button } from "@/components/ui/button"
import { PageContainer } from "@/components/layout/page-container"
import { messages } from "@/lib/messages.ar"
import { ProductActiveFilterChips } from "../blocks/product-active-filter-chips"
import { ProductGrid } from "../blocks/product-grid"
import { ProductSidebar } from "../blocks/product-sidebar"
import { ProductToolbar } from "../blocks/product-toolbar"
import { useStorefrontProducts } from "../../hooks/useProduct"
import { productListingParsers } from "../../search-params"

export function ProductListingPage() {
  const [params, setParams] = useQueryStates(productListingParsers, {
    history: "replace",
    shallow: true,
  })

  const hasAttrs = Object.keys(params.attrs).length > 0

  const { data, isLoading, isFetching } = useStorefrontProducts({
    page: params.page,
    limit: params.limit,
    search: params.search || undefined,
    categoryId: params.categoryId ?? undefined,
    brandIds: params.brandIds.length > 0 ? params.brandIds : undefined,
    minPrice: params.minPrice ?? undefined,
    maxPrice: params.maxPrice ?? undefined,
    inStockOnly: params.inStock === true ? true : undefined,
    attrs: hasAttrs ? params.attrs : undefined,
    sortBy: params.sortBy,
    sortOrder: params.sortOrder,
  })

  const products = data?.items ?? []
  const totalPages = data?.totalPages ?? 1

  return (
    <PageContainer>
      <div className="mb-6">
        <h1 className="text-2xl font-bold">{messages.product.listing.title}</h1>
        <p className="text-muted-foreground mt-1 text-sm">
          {messages.product.listing.description}
        </p>
      </div>

      <div className="flex gap-8">
        <aside className="hidden w-64 shrink-0 lg:block">
          <div className="sticky top-24">
            <ProductSidebar />
          </div>
        </aside>

        <main className="min-w-0 flex-1">
          <ProductToolbar total={data?.total} />
          <ProductActiveFilterChips />

          {products.length === 0 && !isLoading ? (
            <div className="flex flex-col items-center gap-2 py-16 text-center">
              <p className="font-medium">{messages.product.noProducts}</p>
              <p className="text-muted-foreground text-sm">
                {messages.product.noProductsDescription}
              </p>
            </div>
          ) : (
            <ProductGrid products={products} loading={isLoading || isFetching} />
          )}

          {totalPages > 1 && (
            <div className="mt-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <p className="text-muted-foreground text-center text-sm sm:text-start">
                {data
                  ? messages.product.listing.paginationPage(params.page, totalPages)
                  : ""}
              </p>
              <div className="flex justify-center gap-2 sm:justify-end">
                <Button
                  variant="outline"
                  className="h-11 min-w-24"
                  disabled={params.page <= 1}
                  onClick={() => setParams({ page: params.page - 1 })}
                >
                  السابق
                </Button>
                <Button
                  variant="outline"
                  className="h-11 min-w-24"
                  disabled={params.page >= totalPages}
                  onClick={() => setParams({ page: params.page + 1 })}
                >
                  التالي
                </Button>
              </div>
            </div>
          )}
        </main>
      </div>
    </PageContainer>
  )
}
