import type { Metadata } from "next"
import localFont from "next/font/local"
import { NuqsAdapter } from "nuqs/adapters/next/app"

import "./globals.css"
import { AppProviders } from "@/components/app-providers"
import { cn } from "@/lib/utils"

export const metadata: Metadata = {
  title: {
    default: "شركة أكمي — لوحة التحكم",
    template: "%s | شركة أكمي",
  },
  description: "لوحة تحكم الإدارة لشركة أكمي",
  applicationName: "شركة أكمي",
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
