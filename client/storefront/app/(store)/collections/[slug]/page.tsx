import type { Metadata } from "next"
import { notFound } from "next/navigation"
import { JsonLd } from "@/components/seo/json-ld"
import { CollectionPage, getCollectionBySlug } from "@/features/collection"
import { getProductName, messages } from "@/lib/messages.ar"
import { buildSocialMetadata } from "@/lib/open-graph"
import { buildBreadcrumbJsonLd } from "@/lib/structured-data"

export const dynamic = "force-dynamic"

interface CollectionPageProps {
  params: Promise<{ slug: string }>
}

export async function generateMetadata({ params }: CollectionPageProps): Promise<Metadata> {
  const { slug } = await params
  const collection = await getCollectionBySlug(slug)
  if (!collection) return { title: "مجموعة غير موجودة" }

  const name = getProductName(collection.nameAr, collection.nameEn)
  const description = collection.descriptionAr ?? collection.descriptionEn ?? undefined

  return {
    title: name,
    description,
    ...buildSocialMetadata({
      title: name,
      description,
      path: `/collections/${collection.slug}`,
      image: collection.bannerUrl ?? collection.imageUrl,
    }),
  }
}

export default async function CollectionRoutePage({ params }: CollectionPageProps) {
  const { slug } = await params
  if (!slug?.trim()) notFound()

  const collection = await getCollectionBySlug(slug)
  if (!collection) notFound()

  return (
    <>
      <JsonLd
        data={buildBreadcrumbJsonLd([
          { name: messages.common.companyName, path: "/" },
          {
            name: getProductName(collection.nameAr, collection.nameEn),
            path: `/collections/${collection.slug}`,
          },
        ])}
      />
      <CollectionPage collectionId={collection.id} initialCollection={collection} />
    </>
  )
}
