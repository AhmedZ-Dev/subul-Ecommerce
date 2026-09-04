import type { Metadata } from "next"
import { notFound } from "next/navigation"
import { JsonLd } from "@/components/seo/json-ld"
import { ProductDetailPage, getStorefrontProductBySlug } from "@/features/product"
import { getCategoryName, getProductName, messages } from "@/lib/messages.ar"
import { buildSocialMetadata } from "@/lib/open-graph"
import { buildBreadcrumbJsonLd, buildProductJsonLd } from "@/lib/structured-data"

export const dynamic = "force-dynamic"

interface ProductPageProps {
  params: Promise<{ slug: string }>
}

export async function generateMetadata({ params }: ProductPageProps): Promise<Metadata> {
  const { slug } = await params
  const product = await getStorefrontProductBySlug(slug)
  if (!product) return { title: "منتج غير موجود" }

  const name = getProductName(product.nameAr, product.nameEn)
  const description =
    product.shortDescriptionAr ??
    product.descriptionAr ??
    product.metaDescription ??
    product.shortDescriptionEn ??
    product.descriptionEn ??
    undefined

  return {
    title: product.metaTitle ?? name,
    description,
    ...buildSocialMetadata({
      // The Arabic name is what shoppers recognise in a shared link.
      title: name,
      description,
      path: `/products/${product.slug}`,
      image: product.primaryImageUrl,
    }),
  }
}

export default async function ProductPage({ params }: ProductPageProps) {
  const { slug } = await params
  if (!slug?.trim()) notFound()

  const product = await getStorefrontProductBySlug(slug)
  if (!product) notFound()

  const name = getProductName(product.nameAr, product.nameEn)
  const categoryName = product.category
    ? getCategoryName(product.category.nameAr, product.category.nameEn)
    : null

  return (
    <>
      <JsonLd
        data={buildProductJsonLd({
          name,
          description:
            product.shortDescriptionAr ??
            product.descriptionAr ??
            product.metaDescription ??
            product.shortDescriptionEn ??
            product.descriptionEn,
          slug: product.slug,
          sku: product.sku,
          price: product.price,
          currency: product.currency,
          stockQuantity: product.stockQuantity,
          brandName: product.brand?.name,
          categoryName,
          imageUrl: product.primaryImageUrl,
          warrantyMonths: product.warrantyMonths,
        })}
      />
      <JsonLd
        data={buildBreadcrumbJsonLd([
          { name: messages.common.companyName, path: "/" },
          { name: messages.product.listing.title, path: "/products" },
          ...(product.category
            ? [
                {
                  name: categoryName!,
                  path: `/categories/${product.category.slug}`,
                },
              ]
            : []),
          { name, path: `/products/${product.slug}` },
        ])}
      />
      <ProductDetailPage productId={product.id} initialProduct={product} />
    </>
  )
}
