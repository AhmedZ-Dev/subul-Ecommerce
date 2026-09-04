"use client"

import Image from "next/image"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { useState } from "react"
import { Search } from "lucide-react"
import { CartButton } from "@/components/storefront/cart-button"
import { MobileNav } from "@/components/storefront/mobile-nav"
import { MobileSearch } from "@/components/storefront/mobile-search"
import { ThemeToggle } from "@/components/storefront/theme-toggle"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import type { CategoryListItem } from "@/features/category"
import { messages } from "@/lib/messages.ar"
import { getCategoryName } from "@/lib/messages.ar"

interface StorefrontHeaderProps {
  categories: CategoryListItem[]
}

export function StorefrontHeader({ categories }: StorefrontHeaderProps) {
  const router = useRouter()
  const [search, setSearch] = useState("")

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
        <MobileNav categories={categories} />

        <Link
          href="/"
          className="flex min-w-0 shrink-0 items-center gap-2"
        >
          <Image
            src="/assets/logo_subul-brand_full_20260829_black.png"
            alt=""
            width={32}
            height={32}
            className="size-7 shrink-0 object-contain dark:hidden md:size-8"
          />
          <Image
            src="/assets/logo_subul-brand_full_20260829_white.png"
            alt=""
            width={32}
            height={32}
            className="hidden size-7 shrink-0 object-contain dark:block md:size-8"
          />
          <span className="text-primary min-w-0 truncate text-base font-bold md:text-lg">
            {messages.common.companyName}
          </span>
        </Link>

        <nav className="hidden items-center gap-1 lg:flex">
          <Button variant="ghost" size="sm" asChild>
            <Link href="/products">{messages.header.products}</Link>
          </Button>
          {categories.slice(0, 6).map((cat) => (
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
            className="hidden min-w-0 max-w-sm flex-1 items-center lg:flex"
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

          <ThemeToggle />

          <CartButton />
        </div>
      </div>
    </header>
  )
}
