import { AnnouncementBar } from "@/components/storefront/announcement-bar"
import { StorefrontFooter } from "@/components/storefront/footer"
import { StorefrontHeader } from "@/components/storefront/header"

export default function StoreLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <div className="flex min-h-screen flex-col">
      <AnnouncementBar />
      <StorefrontHeader />
      <main className="flex flex-1 flex-col">{children}</main>
      <StorefrontFooter />
    </div>
  )
}
