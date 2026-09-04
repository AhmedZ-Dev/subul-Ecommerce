import { resolveAssetUrl } from "./asset-url"
import { messages } from "./messages.ar"
import { absoluteUrl, getSiteUrl } from "./site-url"

const LOGO_PATH = "/assets/logo_subul-brand_full_20260829_black.png"

/** One step of a breadcrumb trail: a label and the route it points at. */
export interface BreadcrumbStep {
  name: string
  path: string
}

/** Identifies the store itself — used for the knowledge panel and site name. */
export function buildOrganizationJsonLd(): Record<string, unknown> {
  return {
    "@context": "https://schema.org",
    "@type": "Organization",
    name: messages.common.companyName,
    url: getSiteUrl(),
    logo: absoluteUrl(LOGO_PATH),
    contactPoint: [
      {
        "@type": "ContactPoint",
        contactType: "customer support",
        telephone: messages.contact.whatsapp.value,
        areaServed: "IQ",
        availableLanguage: ["ar"],
      },
    ],
  }
}

/** Lets Google offer a search box for the site directly in its results. */
export function buildWebSiteJsonLd(): Record<string, unknown> {
  return {
    "@context": "https://schema.org",
    "@type": "WebSite",
    name: messages.common.companyName,
    url: getSiteUrl(),
    inLanguage: "ar",
    potentialAction: {
      "@type": "SearchAction",
      target: {
        "@type": "EntryPoint",
        urlTemplate: absoluteUrl("/products?search={search_term_string}"),
      },
      "query-input": "required name=search_term_string",
    },
  }
}

/** The trail shown under a search result instead of a bare URL. */
export function buildBreadcrumbJsonLd(steps: BreadcrumbStep[]): Record<string, unknown> {
  return {
    "@context": "https://schema.org",
    "@type": "BreadcrumbList",
    itemListElement: steps.map((step, index) => ({
      "@type": "ListItem",
      position: index + 1,
      name: step.name,
      item: absoluteUrl(step.path),
    })),
  }
}

interface ProductJsonLdInput {
  name: string
  description?: string | null
  slug: string
  sku?: string | null
  price: number
  currency: string
  stockQuantity: number
  brandName?: string | null
  categoryName?: string | null
  imageUrl?: string | null
  warrantyMonths?: number
}

/**
 * Product plus its Offer — this is what lets Google show the price and stock
 * status under the search result rather than a plain text snippet.
 */
export function buildProductJsonLd({
  name,
  description,
  slug,
  sku,
  price,
  currency,
  stockQuantity,
  brandName,
  categoryName,
  imageUrl,
  warrantyMonths,
}: ProductJsonLdInput): Record<string, unknown> {
  const url = absoluteUrl(`/products/${slug}`)
  const image = resolveAssetUrl(imageUrl) ?? absoluteUrl(LOGO_PATH)

  return {
    "@context": "https://schema.org",
    "@type": "Product",
    name,
    ...(description?.trim() ? { description: description.trim() } : {}),
    image: [image],
    ...(sku ? { sku } : {}),
    ...(brandName ? { brand: { "@type": "Brand", name: brandName } } : {}),
    ...(categoryName ? { category: categoryName } : {}),
    offers: {
      "@type": "Offer",
      url,
      price,
      priceCurrency: currency,
      itemCondition: "https://schema.org/NewCondition",
      availability:
        stockQuantity > 0
          ? "https://schema.org/InStock"
          : "https://schema.org/OutOfStock",
      seller: { "@type": "Organization", name: messages.common.companyName },
    },
    ...(warrantyMonths && warrantyMonths > 0
      ? {
          additionalProperty: [
            {
              "@type": "PropertyValue",
              name: messages.product.detail.warranty(warrantyMonths),
              value: `${warrantyMonths}`,
            },
          ],
        }
      : {}),
  }
}
