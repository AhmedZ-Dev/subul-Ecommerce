"use client"

import Link from "next/link"
import { Menu } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet"
import { useCategoryNav } from "@/features/category"
import { messages, getCategoryName } from "@/lib/messages.ar"

export function MobileNav() {
  const { data: categories } = useCategoryNav()

  return (
    <Sheet>
      <SheetTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          className="size-11 md:hidden"
          aria-label={messages.header.menu}
        >
          <Menu className="size-5" />
        </Button>
      </SheetTrigger>
      <SheetContent side="right" className="w-72">
        <SheetHeader>
          <SheetTitle>{messages.common.companyName}</SheetTitle>
        </SheetHeader>
        <nav className="mt-6 flex flex-col gap-1">
          <Button variant="ghost" className="h-11 justify-start" asChild>
            <Link href="/">{messages.common.home}</Link>
          </Button>
          <Button variant="ghost" className="h-11 justify-start" asChild>
            <Link href="/products">{messages.header.products}</Link>
          </Button>
          {categories?.map((cat) => (
            <Button key={cat.id} variant="ghost" className="h-11 justify-start" asChild>
              <Link href={`/categories/${cat.slug}`}>
                {getCategoryName(cat.nameAr, cat.nameEn)}
              </Link>
            </Button>
          ))}
          <Button variant="ghost" className="h-11 justify-start" asChild>
            <Link href="/orders/track">{messages.footer.trackOrder}</Link>
          </Button>
        </nav>
      </SheetContent>
    </Sheet>
  )
}
