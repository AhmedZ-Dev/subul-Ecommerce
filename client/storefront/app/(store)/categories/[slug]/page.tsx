import type { Metadata } from "next"
import { notFound } from "next/navigation"
import { CategoryListingPage, getStorefrontCategoryBySlug } from "@/features/category"
import { getCategoryName } from "@/lib/messages.ar"

export const dynamic = "force-dynamic"

interface CategoryPageProps {
  params: Promise<{ slug: string }>
}

export async function generateMetadata({ params }: CategoryPageProps): Promise<Metadata> {
  const { slug } = await params
  const category = await getStorefrontCategoryBySlug(slug)
  if (!category) return { title: "تصنيف غير موجود" }

  return {
    title: getCategoryName(category.nameAr, category.nameEn),
  }
}

export default async function CategoryPage({ params }: CategoryPageProps) {
  const { slug } = await params
  if (!slug?.trim()) notFound()

  const category = await getStorefrontCategoryBySlug(slug)
  if (!category) notFound()

  return <CategoryListingPage categoryId={category.id} />
}
