import type { Metadata } from "next"
import { notFound } from "next/navigation"
import { JsonLd } from "@/components/seo/json-ld"
import { CategoryListingPage, getStorefrontCategoryBySlug } from "@/features/category"
import { getCategoryName, messages } from "@/lib/messages.ar"
import { buildSocialMetadata } from "@/lib/open-graph"
import { buildBreadcrumbJsonLd } from "@/lib/structured-data"

export const dynamic = "force-dynamic"

interface CategoryPageProps {
  params: Promise<{ slug: string }>
}

export async function generateMetadata({ params }: CategoryPageProps): Promise<Metadata> {
  const { slug } = await params
  const category = await getStorefrontCategoryBySlug(slug)
  if (!category) return { title: "تصنيف غير موجود" }

  const name = getCategoryName(category.nameAr, category.nameEn)
  const description = category.descriptionAr ?? category.descriptionEn ?? undefined

  return {
    title: name,
    description,
    ...buildSocialMetadata({
      title: name,
      description,
      path: `/categories/${category.slug}`,
      image: category.imageUrl,
    }),
  }
}

export default async function CategoryPage({ params }: CategoryPageProps) {
  const { slug } = await params
  if (!slug?.trim()) notFound()

  const category = await getStorefrontCategoryBySlug(slug)
  if (!category) notFound()

  return (
    <>
      <JsonLd
        data={buildBreadcrumbJsonLd([
          { name: messages.common.companyName, path: "/" },
          { name: messages.product.listing.title, path: "/products" },
          {
            name: getCategoryName(category.nameAr, category.nameEn),
            path: `/categories/${category.slug}`,
          },
        ])}
      />
      <CategoryListingPage categoryId={category.id} />
    </>
  )
}
