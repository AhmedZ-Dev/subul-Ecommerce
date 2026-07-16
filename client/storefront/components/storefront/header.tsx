"use client"

import Link from "next/link"
import { useRouter } from "next/navigation"
import { useState } from "react"
import { Search } from "lucide-react"
import { CartButton } from "@/components/storefront/cart-button"
import { MobileNav } from "@/components/storefront/mobile-nav"
import { MobileSearch } from "@/components/storefront/mobile-search"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { useCategoryNav } from "@/features/category"
import { messages } from "@/lib/messages.ar"
import { getCategoryName } from "@/lib/messages.ar"

export function StorefrontHeader() {
  const router = useRouter()
  const [search, setSearch] = useState("")
  const { data: categories } = useCategoryNav()

  function handleSearch(e: React.FormEvent) {
    e.preventDefault()
    const q = search.trim()
    if (q) {
      router.push(`/products?search=${encodeURIComponent(q)}`)
    } else {
      router.push("/products")
    }
  }

  return (
    <header className="bg-background/95 supports-[backdrop-filter]:bg-background/60 sticky top-0 z-50 border-b backdrop-blur">
      <div className="container mx-auto flex h-14 items-center gap-2 px-4 md:h-16 md:gap-4 md:px-6">
        <MobileNav />

        <Link
          href="/"
          className="text-primary min-w-0 truncate text-base font-bold md:text-lg"
        >
          {messages.common.companyName}
        </Link>

        <nav className="hidden items-center gap-1 md:flex">
          <Button variant="ghost" size="sm" asChild>
            <Link href="/products">{messages.header.products}</Link>
          </Button>
          {categories?.slice(0, 6).map((cat) => (
            <Button key={cat.id} variant="ghost" size="sm" asChild>
              <Link href={`/categories/${cat.slug}`}>
                {getCategoryName(cat.nameAr, cat.nameEn)}
              </Link>
            </Button>
          ))}
        </nav>

        <div className="ms-auto flex min-w-0 items-center gap-1 md:gap-2">
          <MobileSearch />

          <form
            onSubmit={handleSearch}
            className="hidden min-w-0 max-w-sm flex-1 items-center md:flex"
          >
            <div className="relative w-full">
              <Search className="text-muted-foreground absolute start-3 top-1/2 size-4 -translate-y-1/2" />
              <Input
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder={messages.header.searchPlaceholder}
                className="ps-9"
              />
            </div>
          </form>

          <CartButton />
        </div>
      </div>
    </header>
  )
}
