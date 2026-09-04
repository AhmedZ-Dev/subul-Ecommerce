import { AnnouncementBar } from "@/components/storefront/announcement-bar"
import { StorefrontFooter } from "@/components/storefront/footer"
import { StorefrontHeader } from "@/components/storefront/header"
import { getCachedTopLevelCategories, type CategoryListItem } from "@/features/category"

export default async function StoreLayout({
  children,
}: {
  children: React.ReactNode
}) {
  // Fetched here rather than in the header itself: the nav then ships inside the
  // first HTML response instead of popping in — and every page under this layout
  // shares the one request.
  let categories: CategoryListItem[] = []
  try {
    categories = await getCachedTopLevelCategories()
  } catch {
    // API unavailable — render the header without category links.
  }

  return (
    <div className="flex min-h-screen flex-col">
      <AnnouncementBar />
      <StorefrontHeader categories={categories} />
      <main className="flex flex-1 flex-col">{children}</main>
      <StorefrontFooter />
    </div>
  )
}
