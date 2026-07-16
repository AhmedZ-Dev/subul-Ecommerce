"use client"

import { useRouter } from "next/navigation"
import { useState } from "react"
import { Search } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet"
import { messages } from "@/lib/messages.ar"

export function MobileSearch() {
  const router = useRouter()
  const [open, setOpen] = useState(false)
  const [search, setSearch] = useState("")

  function handleSearch(e: React.FormEvent) {
    e.preventDefault()
    const q = search.trim()
    setOpen(false)
    if (q) {
      router.push(`/products?search=${encodeURIComponent(q)}`)
    } else {
      router.push("/products")
    }
  }

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          className="size-11 md:hidden"
          aria-label={messages.header.searchPlaceholder}
        >
          <Search className="size-5" />
        </Button>
      </SheetTrigger>
      <SheetContent side="top" className="px-4 pt-6">
        <SheetHeader>
          <SheetTitle>{messages.header.searchPlaceholder}</SheetTitle>
        </SheetHeader>
        <form onSubmit={handleSearch} className="mt-4">
          <div className="relative">
            <Search className="text-muted-foreground absolute start-3 top-1/2 size-4 -translate-y-1/2" />
            <Input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder={messages.header.searchPlaceholder}
              className="h-11 ps-9 text-base"
              autoFocus
            />
          </div>
          <Button type="submit" className="mt-4 h-11 w-full">
            {messages.product.filters.search}
          </Button>
        </form>
      </SheetContent>
    </Sheet>
  )
}
