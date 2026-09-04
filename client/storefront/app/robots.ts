import type { MetadataRoute } from "next"
import { absoluteUrl, getSiteUrl } from "@/lib/site-url"

export default function robots(): MetadataRoute.Robots {
  return {
    rules: [
      {
        userAgent: "*",
        allow: "/",
        // Per-visitor pages with nothing to index.
        disallow: ["/cart", "/checkout", "/order-confirmation"],
      },
    ],
    sitemap: absoluteUrl("/sitemap.xml"),
    host: getSiteUrl(),
  }
}
