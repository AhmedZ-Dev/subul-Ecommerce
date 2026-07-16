import type { Metadata } from "next"
import { notFound } from "next/navigation"
import { ProductDetailPage, getStorefrontProductBySlug } from "@/features/product"
import { getProductName } from "@/lib/messages.ar"

export const dynamic = "force-dynamic"

interface ProductPageProps {
  params: Promise<{ slug: string }>
}

export async function generateMetadata({ params }: ProductPageProps): Promise<Metadata> {
  const { slug } = await params
  const product = await getStorefrontProductBySlug(slug)
  if (!product) return { title: "منتج غير موجود" }

  const name = getProductName(product.nameAr, product.nameEn)
  return {
    title: product.metaTitle ?? name,
    description: product.metaDescription ?? product.shortDescriptionAr ?? product.shortDescriptionEn ?? undefined,
  }
}

export default async function ProductPage({ params }: ProductPageProps) {
  const { slug } = await params
  if (!slug?.trim()) notFound()

  const product = await getStorefrontProductBySlug(slug)
  if (!product) notFound()

  return <ProductDetailPage productId={product.id} initialProduct={product} />
}
