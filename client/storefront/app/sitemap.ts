import type { MetadataRoute } from "next"
import { getStorefrontCategories } from "@/features/category"
import { getActiveCollections } from "@/features/collection"
import { getStorefrontProducts } from "@/features/product"
import { absoluteUrl } from "@/lib/site-url"

// Built per request so a newly published product is discoverable immediately.
export const dynamic = "force-dynamic"

const PRODUCT_PAGE_SIZE = 200
const MAX_PRODUCT_PAGES = 25

/** Walks the paginated catalog so the sitemap is not capped at one page. */
async function getAllProductSlugs(): Promise<string[]> {
  const slugs: string[] = []

  for (let page = 1; page <= MAX_PRODUCT_PAGES; page += 1) {
    const result = await getStorefrontProducts({ page, limit: PRODUCT_PAGE_SIZE })
    slugs.push(...result.items.map((product) => product.slug))
    if (page >= result.totalPages) break
  }

  return slugs
}

export default async function sitemap(): Promise<MetadataRoute.Sitemap> {
  const staticRoutes: MetadataRoute.Sitemap = [
    { url: absoluteUrl("/"), changeFrequency: "daily", priority: 1 },
    { url: absoluteUrl("/products"), changeFrequency: "daily", priority: 0.9 },
    { url: absoluteUrl("/orders/track"), changeFrequency: "yearly", priority: 0.3 },
  ]

  try {
    const [productSlugs, categories, collections] = await Promise.all([
      getAllProductSlugs(),
      getStorefrontCategories({ limit: 500, isActive: true }).then((r) => r.items),
      getActiveCollections(),
    ])

    return [
      ...staticRoutes,
      ...categories.map((category) => ({
        url: absoluteUrl(`/categories/${category.slug}`),
        changeFrequency: "weekly" as const,
        priority: 0.8,
      })),
      ...collections.map((collection) => ({
        url: absoluteUrl(`/collections/${collection.slug}`),
        changeFrequency: "weekly" as const,
        priority: 0.7,
      })),
      ...productSlugs.map((slug) => ({
        url: absoluteUrl(`/products/${slug}`),
        changeFrequency: "weekly" as const,
        priority: 0.7,
      })),
    ]
  } catch {
    // API unreachable — still serve a valid sitemap of the static routes.
    return staticRoutes
  }
}
