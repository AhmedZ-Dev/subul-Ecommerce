import type { Metadata } from "next"
import localFont from "next/font/local"
import { NuqsAdapter } from "nuqs/adapters/next/app"

import "./globals.css"
import { AppProviders } from "@/components/app-providers"
import { messages } from "@/lib/messages.ar"
import { getSiteUrl } from "@/lib/site-url"
import { cn } from "@/lib/utils"

const companyName = messages.common.companyName
const siteUrl = getSiteUrl()
const defaultTitle = `${companyName} — المتجر`
const defaultDescription = `تسوق أونلاين من ${companyName}`
const defaultImage = "/assets/logo_subul-brand_full_20260829_black.png"

export const metadata: Metadata = {
  // Lets pages declare relative image and canonical paths.
  metadataBase: new URL(siteUrl),
  title: {
    default: defaultTitle,
    template: `%s | ${companyName}`,
  },
  description: defaultDescription,
  applicationName: companyName,
  // Inherited by any page that does not set its own preview card.
  openGraph: {
    type: "website",
    url: siteUrl,
    siteName: companyName,
    locale: "ar_IQ",
    title: defaultTitle,
    description: defaultDescription,
    images: [{ url: defaultImage, alt: companyName }],
  },
  twitter: {
    card: "summary_large_image",
    title: defaultTitle,
    description: defaultDescription,
    images: [defaultImage],
  },
  icons: {
    icon: [
      {
        url: "/assets/logo_subul-brand_full_20260829_black.png",
        media: "(prefers-color-scheme: light)",
      },
      {
        url: "/assets/logo_subul-brand_full_20260829_white.png",
        media: "(prefers-color-scheme: dark)",
      },
    ],
    apple: "/assets/logo_subul-brand_full_20260829_black.png",
  },
}

const fontSans = localFont({
  src: [
    {
      path: "./fonts/cairo/Cairo-Variable.woff2",
      weight: "200 1000",
      style: "normal",
    },
    {
      path: "./fonts/cairo/Cairo-Latin-Variable.woff2",
      weight: "200 1000",
      style: "normal",
    },
  ],
  variable: "--font-sans",
  display: "swap",
})

const fontMono = localFont({
  src: "./fonts/geist-mono/GeistMono-Variable.woff2",
  variable: "--font-mono",
  weight: "100 900",
  display: "swap",
})

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <html
      lang="ar"
      dir="rtl"
      suppressHydrationWarning
      className={cn(
        "antialiased font-sans",
        fontSans.variable,
        fontMono.variable
      )}
    >
      <body>
        <NuqsAdapter>
          <AppProviders>{children}</AppProviders>
        </NuqsAdapter>
      </body>
    </html>
  )
}
