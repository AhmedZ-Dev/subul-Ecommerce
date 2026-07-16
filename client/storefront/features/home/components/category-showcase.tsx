import Link from "next/link"
import Image from "next/image"
import { getCategoryName, messages } from "@/lib/messages.ar"
import { resolveAssetUrl } from "@/lib/asset-url"
import type { CategoryListItem } from "@/features/category"
import { SectionHeader } from "./section-header"

interface CategoryShowcaseProps {
  categories: CategoryListItem[]
}

export function CategoryShowcase({ categories }: CategoryShowcaseProps) {
  if (categories.length === 0) return null

  return (
    <section className="mb-14 md:mb-16" aria-label={messages.storefront.topCategories}>
      <SectionHeader
        title={messages.storefront.topCategories}
        description={messages.storefront.topCategoriesDescription}
        viewAllHref="/products"
      />

      <div className="grid grid-cols-2 gap-3 sm:gap-4 md:grid-cols-4 lg:grid-cols-5">
        {categories.map((category) => {
          const name = getCategoryName(category.nameAr, category.nameEn)
          const imageUrl = resolveAssetUrl(category.imageUrl)

          return (
            <Link
              key={category.id}
              href={`/categories/${category.slug}`}
              className="group relative overflow-hidden rounded-2xl ring-1 ring-foreground/8 transition-[transform,box-shadow,ring-color] duration-300 hover:-translate-y-0.5 hover:shadow-lg hover:ring-primary/25"
            >
              <div className="relative aspect-4/3 overflow-hidden">
                {imageUrl ? (
                  <Image
                    src={imageUrl}
                    alt={name}
                    fill
                    className="object-cover transition-transform duration-500 group-hover:scale-105"
                    sizes="(max-width: 768px) 50vw, 20vw"
                  />
                ) : (
                  <div className="hero-gradient flex h-full items-center justify-center opacity-80">
                    <span className="text-primary-foreground text-3xl font-bold">
                      {name.charAt(0)}
                    </span>
                  </div>
                )}
                <div className="absolute inset-0 bg-linear-to-t from-black/80 via-black/30 to-transparent" />
                <div className="absolute inset-x-0 bottom-0 space-y-1 p-3 sm:p-3.5">
                  <p className="line-clamp-2 text-sm font-semibold text-white sm:text-[0.9375rem]">
                    {name}
                  </p>
                </div>
              </div>
            </Link>
          )
        })}
      </div>
    </section>
  )
}
