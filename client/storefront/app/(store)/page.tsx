import type { Metadata } from "next"
import { Suspense } from "react"
import { getTopLevelCategories } from "@/features/category"
import { getActiveCollections } from "@/features/collection"
import { getStorefrontProducts } from "@/features/product"
import { HomePageContent } from "@/features/home/components/home-page-content"
import type { CategoryProductSection } from "@/features/home/components/home-page-content"
import { messages } from "@/lib/messages.ar"

export const dynamic = "force-dynamic"

export const metadata: Metadata = {
  title: messages.storefront.pageTitle,
  description: messages.storefront.description,
}

const MAX_CATEGORY_SECTIONS = 4
const PRODUCTS_PER_CATEGORY = 8
const FEATURED_PRODUCTS_LIMIT = 8

export default async function HomePage() {
  let topCategories: Awaited<ReturnType<typeof getTopLevelCategories>> = []
  let featuredProducts: Awaited<ReturnType<typeof getStorefrontProducts>>["items"] = []
  let collections: Awaited<ReturnType<typeof getActiveCollections>> = []
  let categoryProductSections: CategoryProductSection[] = []

  try {
    const [categories, featured, activeCollections] = await Promise.all([
      getTopLevelCategories(),
      getStorefrontProducts({
        isFeatured: true,
        limit: FEATURED_PRODUCTS_LIMIT,
        sortBy: "createdAt",
        sortOrder: "desc",
      }).catch(() => ({ items: [] })),
      getActiveCollections().catch(() => []),
    ])

    topCategories = categories
    featuredProducts = featured.items
    collections = activeCollections.slice(0, 6)

    const sectionsToFetch = topCategories.slice(0, MAX_CATEGORY_SECTIONS)
    const sectionResults = await Promise.all(
      sectionsToFetch.map((category) =>
        getStorefrontProducts({
          categoryId: category.id,
          limit: PRODUCTS_PER_CATEGORY,
          sortBy: "createdAt",
          sortOrder: "desc",
        })
          .then((result) => ({
            category,
            products: result.items,
          }))
          .catch(() => ({
            category,
            products: [],
          })),
      ),
    )

    categoryProductSections = sectionResults.filter(
      (section) => section.products.length > 0,
    )
  } catch {
    // API unavailable — render empty homepage shell
  }

  return (
    <Suspense fallback={<div className="p-8 text-center">{messages.common.loading}</div>}>
      <HomePageContent
        topCategories={topCategories}
        featuredProducts={featuredProducts}
        collections={collections}
        categoryProductSections={categoryProductSections}
      />
    </Suspense>
  )
}
