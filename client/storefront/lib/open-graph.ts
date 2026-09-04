import type { Metadata } from "next"
import { resolveAssetUrl } from "./asset-url"
import { messages } from "./messages.ar"
import { absoluteUrl, getSiteUrl } from "./site-url"

/** Shown when a page has no image of its own. */
const FALLBACK_IMAGE = "/assets/logo_subul-brand_full_20260829_black.png"

interface SocialMetadataInput {
  title: string
  description?: string | null
  /** Route path starting with "/" — becomes both og:url and the canonical. */
  path: string
  /** Stored asset path or absolute URL of the image to preview. */
  image?: string | null
}

/**
 * The canonical link plus the og:/twitter: tags WhatsApp, Facebook and Telegram
 * read to build a link preview card. Without them a shared link is bare text.
 */
export function buildSocialMetadata({
  title,
  description,
  path,
  image,
}: SocialMetadataInput): Metadata {
  const url = absoluteUrl(path)
  const previewImage = resolveAssetUrl(image) ?? `${getSiteUrl()}${FALLBACK_IMAGE}`
  const summary = description?.trim() || undefined

  return {
    alternates: { canonical: url },
    openGraph: {
      type: "website",
      url,
      title,
      description: summary,
      siteName: messages.common.companyName,
      locale: "ar_IQ",
      images: [{ url: previewImage, alt: title }],
    },
    twitter: {
      card: "summary_large_image",
      title,
      description: summary,
      images: [previewImage],
    },
  }
}
