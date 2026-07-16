"use client"

import { useQueryStates } from "nuqs"
import { Button } from "@/components/ui/button"
import { PageContainer } from "@/components/layout/page-container"
import { CategoryBanner } from "@/components/storefront/category-banner"
import {
  ProductActiveFilterChips,
  ProductGrid,
  ProductSidebar,
  ProductToolbar,
  productListingParsers,
  useStorefrontProducts,
} from "@/features/product"
import { getCategoryName, messages } from "@/lib/messages.ar"
import { useStorefrontCategory } from "../../hooks/useCategory"

interface CategoryListingPageProps {
  categoryId: number
}

export function CategoryListingPage({ categoryId }: CategoryListingPageProps) {
  const { data: category } = useStorefrontCategory(categoryId)
  const [params, setParams] = useQueryStates(productListingParsers, {
    history: "replace",
    shallow: true,
  })

  const hasAttrs = Object.keys(params.attrs).length > 0

  const { data, isLoading, isFetching } = useStorefrontProducts({
    page: params.page,
    limit: params.limit,
    categoryId,
    brandIds: params.brandIds.length > 0 ? params.brandIds : undefined,
    minPrice: params.minPrice ?? undefined,
    maxPrice: params.maxPrice ?? undefined,
    inStockOnly: params.inStock === true ? true : undefined,
    attrs: hasAttrs ? params.attrs : undefined,
    sortBy: params.sortBy,
    sortOrder: params.sortOrder,
  })

  const products = data?.items ?? []
  const name = category
    ? getCategoryName(category.nameAr, category.nameEn)
    : messages.category.listing.title

  return (
    <PageContainer>
      <CategoryBanner
        title={name}
        description={messages.category.listing.productsInCategory}
      />

      <div className="flex gap-8">
        <aside className="hidden w-64 shrink-0 lg:block">
          <div className="sticky top-24">
            <ProductSidebar categoryId={categoryId} />
          </div>
        </aside>

        <main className="min-w-0 flex-1">
          <ProductToolbar total={data?.total} categoryId={categoryId} />
          <ProductActiveFilterChips categoryId={categoryId} />

          {products.length === 0 && !isLoading ? (
            <div className="flex flex-col items-center gap-2 py-16 text-center">
              <p className="font-medium">{messages.category.listing.emptyTitle}</p>
              <p className="text-muted-foreground text-sm">
                {messages.category.listing.emptyDescription}
              </p>
            </div>
          ) : (
            <ProductGrid products={products} loading={isLoading || isFetching} />
          )}

          {(data?.totalPages ?? 1) > 1 && (
            <div className="mt-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <p className="text-muted-foreground text-center text-sm sm:text-start">
                {data
                  ? messages.product.listing.paginationPage(
                      params.page,
                      data.totalPages ?? 1,
                    )
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
                  disabled={params.page >= (data?.totalPages ?? 1)}
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
