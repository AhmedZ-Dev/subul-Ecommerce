import type { Metadata } from "next"
import { notFound } from "next/navigation"
import { CollectionPage, getCollectionBySlug } from "@/features/collection"
import { getProductName } from "@/lib/messages.ar"

export const dynamic = "force-dynamic"

interface CollectionPageProps {
  params: Promise<{ slug: string }>
}

export async function generateMetadata({ params }: CollectionPageProps): Promise<Metadata> {
  const { slug } = await params
  const collection = await getCollectionBySlug(slug)
  if (!collection) return { title: "مجموعة غير موجودة" }

  return {
    title: getProductName(collection.nameAr, collection.nameEn),
  }
}

export default async function CollectionRoutePage({ params }: CollectionPageProps) {
  const { slug } = await params
  if (!slug?.trim()) notFound()

  const collection = await getCollectionBySlug(slug)
  if (!collection) notFound()

  return <CollectionPage collectionId={collection.id} initialCollection={collection} />
}
