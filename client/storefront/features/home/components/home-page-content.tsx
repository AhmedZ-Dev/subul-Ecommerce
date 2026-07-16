import { PageContainer } from "@/components/layout/page-container"
import type { CategoryListItem } from "@/features/category"
import type { CollectionListItem } from "@/features/collection"
import type { StorefrontProductListItem } from "@/features/product"
import { getCategoryName } from "@/lib/messages.ar"
import { BrandStrip } from "./brand-strip"
import { CategoryProductsRow } from "./category-products-row"
import { CategoryShowcase } from "./category-showcase"
import { CollectionsShowcase } from "./collections-showcase"
import { ContactStrip } from "./contact-strip"
import { FeaturedProductsSection } from "./featured-products-section"
import { HeroCarousel } from "./hero-carousel"
import { PromoBanner } from "./promo-banner"
import { TrustStrip } from "./trust-strip"

export interface CategoryProductSection {
  category: CategoryListItem
  products: StorefrontProductListItem[]
}

interface HomePageContentProps {
  topCategories: CategoryListItem[]
  featuredProducts: StorefrontProductListItem[]
  collections: CollectionListItem[]
  categoryProductSections: CategoryProductSection[]
}

function HomePageContent({
  topCategories,
  featuredProducts,
  collections,
  categoryProductSections,
}: HomePageContentProps) {
  // Keep the page focused: hero → browse → featured → trust → collections → one promo → a few rows
  const limitedCategorySections = categoryProductSections.slice(0, 2)

  return (
    <PageContainer>
      <HeroCarousel />

      <CategoryShowcase categories={topCategories} />

      <FeaturedProductsSection products={featuredProducts} />

      <TrustStrip />

      <CollectionsShowcase collections={collections} />

      <PromoBanner />

      {limitedCategorySections.map((section) => (
        <CategoryProductsRow
          key={section.category.id}
          title={getCategoryName(section.category.nameAr, section.category.nameEn)}
          viewAllHref={`/categories/${section.category.slug}`}
          products={section.products}
        />
      ))}

      <BrandStrip />

      <ContactStrip />
    </PageContainer>
  )
}

export { HomePageContent }
