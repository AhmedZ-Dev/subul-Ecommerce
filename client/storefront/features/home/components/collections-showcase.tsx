import Link from "next/link"
import Image from "next/image"
import { ArrowLeft } from "lucide-react"
import { getCategoryName, messages } from "@/lib/messages.ar"
import { resolveAssetUrl } from "@/lib/asset-url"
import type { CollectionListItem } from "@/features/collection"
import { SectionHeader } from "./section-header"
import { cn } from "@/lib/utils"

interface CollectionsShowcaseProps {
  collections: CollectionListItem[]
}

export function CollectionsShowcase({ collections }: CollectionsShowcaseProps) {
  if (collections.length === 0) return null

  const [featured, ...rest] = collections

  return (
    <section className="mb-14 md:mb-16" aria-label={messages.storefront.collections}>
      <SectionHeader
        title={messages.storefront.collections}
        description={messages.storefront.collectionsDescription}
      />

      <div className="grid grid-cols-1 gap-3 sm:gap-4 lg:grid-cols-3">
        {featured && (
          <CollectionTile
            collection={featured}
            featured
            className="lg:col-span-2 lg:row-span-2"
          />
        )}

        <div
          className={cn(
            "grid gap-3 sm:gap-4",
            rest.length > 0 ? "grid-cols-1 sm:grid-cols-2 lg:grid-cols-1" : "",
          )}
        >
          {rest.slice(0, 4).map((collection) => (
            <CollectionTile key={collection.id} collection={collection} />
          ))}
        </div>
      </div>
    </section>
  )
}

function CollectionTile({
  collection,
  featured = false,
  className,
}: {
  collection: CollectionListItem
  featured?: boolean
  className?: string
}) {
  const name = getCategoryName(collection.nameAr, collection.nameEn)
  const imageUrl = resolveAssetUrl(collection.imageUrl)

  return (
    <Link
      href={`/collections/${collection.slug}`}
      className={cn(
        "group relative overflow-hidden rounded-2xl ring-1 ring-foreground/8 transition-[transform,box-shadow,ring-color] duration-300 hover:-translate-y-0.5 hover:shadow-lg hover:ring-primary/25",
        className,
      )}
    >
      <div
        className={cn(
          "relative overflow-hidden",
          featured ? "aspect-16/10 min-h-56 lg:aspect-auto lg:h-full lg:min-h-88" : "aspect-16/10",
        )}
      >
        {imageUrl ? (
          <Image
            src={imageUrl}
            alt={name}
            fill
            className="object-cover transition-transform duration-500 group-hover:scale-105"
            sizes={featured ? "(max-width: 1024px) 100vw, 66vw" : "(max-width: 1024px) 50vw, 33vw"}
          />
        ) : (
          <div className="hero-gradient flex h-full min-h-40 items-center justify-center">
            <span className="text-primary-foreground text-2xl font-bold">
              {name.charAt(0)}
            </span>
          </div>
        )}
        <div className="absolute inset-0 bg-linear-to-t from-black/85 via-black/35 to-black/10" />
        <div className="absolute inset-x-0 bottom-0 flex flex-col gap-2 p-4 sm:p-5">
          <p
            className={cn(
              "line-clamp-2 font-semibold text-white",
              featured ? "text-xl sm:text-2xl" : "text-base",
            )}
          >
            {name}
          </p>
          <div className="flex items-center justify-between gap-3">
            <p className="text-sm text-white/80">
              {messages.storefront.productCount(collection.productCount)}
            </p>
            {featured && (
              <span className="inline-flex items-center gap-1 text-sm font-medium text-white">
                {messages.storefront.exploreCollection}
                <ArrowLeft className="size-4" aria-hidden />
              </span>
            )}
          </div>
        </div>
      </div>
    </Link>
  )
}
