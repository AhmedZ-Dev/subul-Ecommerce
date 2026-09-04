"use client"

import Link from "next/link"
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
import { useStorefrontCategories, useStorefrontCategory } from "../../hooks/useCategory"

interface CategoryListingPageProps {
  categoryId: number
}

export function CategoryListingPage({ categoryId }: CategoryListingPageProps) {
  const { data: category } = useStorefrontCategory(categoryId)
  const [params, setParams] = useQueryStates(productListingParsers, {
    history: "replace",
    shallow: true,
  })

  const { data: subCategoriesData } = useStorefrontCategories({
    parentId: categoryId,
    isActive: true,
    limit: 100,
    sortBy: "sortOrder",
    sortOrder: "asc",
  })
  const subCategories = subCategoriesData?.items ?? []

  const hasAttrs = Object.keys(params.attrs).length > 0

  const { data, isLoading, isFetching, isError, refetch } = useStorefrontProducts({
    page: params.page,
    limit: params.limit,
    search: params.search || undefined,
    categoryId,
    // Parent categories rarely hold products directly — browsing one has to
    // show everything filed under its subcategories.
    includeDescendants: true,
    brandIds: params.brandIds.length > 0 ? params.brandIds : undefined,
    minPrice: params.minPrice ?? undefined,
    maxPrice: params.maxPrice ?? undefined,
    inStockOnly: params.inStock === true ? true : undefined,
    attrs: hasAttrs ? params.attrs : undefined,
    sortBy: params.sortBy,
    sortOrder: params.sortOrder,
  })

  const products = data?.items ?? []

  const hasActiveFilters =
    Boolean(params.search) ||
    params.brandIds.length > 0 ||
    params.brandId != null ||
    params.minPrice != null ||
    params.maxPrice != null ||
    params.inStock != null ||
    hasAttrs

  const resetFilters = () =>
    setParams({
      search: null,
      brandIds: [],
      brandId: null,
      minPrice: null,
      maxPrice: null,
      inStock: null,
      attrs: {},
      page: 1,
    })

  const name = category
    ? getCategoryName(category.nameAr, category.nameEn)
    : messages.category.listing.title

  return (
    <PageContainer>
      <CategoryBanner
        title={name}
        description={messages.category.listing.productsInCategory}
      />

      {subCategories.length > 0 && (
        <nav className="mb-6" aria-label={messages.category.listing.subCategories}>
          <p className="text-muted-foreground mb-2 text-sm font-medium">
            {messages.category.listing.subCategories}
          </p>
          <div className="flex flex-wrap gap-2">
            {subCategories.map((sub) => (
              <Link
                key={sub.id}
                href={`/categories/${sub.slug}`}
                className="bg-card hover:border-primary/40 hover:text-primary flex min-h-10 items-center rounded-full border border-foreground/10 px-4 text-sm font-medium transition-colors"
              >
                {getCategoryName(sub.nameAr, sub.nameEn)}
              </Link>
            ))}
          </div>
        </nav>
      )}

      <div className="flex gap-8">
        <aside className="hidden w-64 shrink-0 lg:block">
          <div className="sticky top-24">
            <ProductSidebar categoryId={categoryId} includeDescendants />
          </div>
        </aside>

        <main className="min-w-0 flex-1">
          <ProductToolbar
            total={data?.total}
            categoryId={categoryId}
            includeDescendants
          />
          <ProductActiveFilterChips categoryId={categoryId} includeDescendants />

          {isError ? (
            <div className="flex flex-col items-center gap-2 py-16 text-center">
              <p className="font-medium">{messages.product.loadError}</p>
              <p className="text-muted-foreground text-sm">
                {messages.product.loadErrorDescription}
              </p>
              <Button variant="outline" className="mt-3 h-11" onClick={() => refetch()}>
                {messages.common.retry}
              </Button>
            </div>
          ) : products.length === 0 && !isLoading ? (
            <div className="flex flex-col items-center gap-2 py-16 text-center">
              <p className="font-medium">{messages.category.listing.emptyTitle}</p>
              <p className="text-muted-foreground text-sm">
                {messages.category.listing.emptyDescription}
              </p>
              {hasActiveFilters && (
                <Button variant="outline" className="mt-3 h-11" onClick={resetFilters}>
                  {messages.product.filters.clearFilters}
                </Button>
              )}
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
