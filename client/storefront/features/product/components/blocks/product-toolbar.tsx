"use client"

import { SlidersHorizontal } from "lucide-react"
import { useQueryStates } from "nuqs"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet"
import { messages } from "@/lib/messages.ar"
import { ProductSidebar } from "./product-sidebar"
import { productListingParsers } from "../../search-params"

interface ProductToolbarProps {
  total?: number
  categoryId?: number
}

export function ProductToolbar({ total, categoryId }: ProductToolbarProps) {
  const [params, setParams] = useQueryStates(productListingParsers, {
    history: "replace",
    shallow: true,
  })

  return (
    <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-center gap-3">
        <Sheet>
          <SheetTrigger asChild>
            <Button variant="outline" className="h-10 gap-2 lg:hidden">
              <SlidersHorizontal className="size-4" />
              {messages.product.filters.title}
            </Button>
          </SheetTrigger>
          <SheetContent
            side="right"
            className="flex h-full max-h-dvh w-full max-w-sm flex-col gap-0 overflow-hidden p-0"
          >
            <SheetHeader className="border-border shrink-0 border-b px-4 py-4 pe-14">
              <SheetTitle>{messages.product.filters.title}</SheetTitle>
            </SheetHeader>
            <div className="min-h-0 flex-1 touch-pan-y overflow-y-auto overscroll-contain px-4 pb-[max(1.5rem,env(safe-area-inset-bottom))] [-webkit-overflow-scrolling:touch]">
              <ProductSidebar categoryId={categoryId} variant="sheet" />
            </div>
          </SheetContent>
        </Sheet>

        <p className="text-muted-foreground text-sm">
          {total != null
            ? messages.product.listing.paginationTotal(total)
            : messages.common.loading}
        </p>
      </div>

      <div className="w-full sm:w-52">
        <Label className="sr-only">{messages.product.filters.sortBy}</Label>
        <Select
          value={`${params.sortBy}-${params.sortOrder}`}
          onValueChange={(value) => {
            const [sortBy, sortOrder] = value.split("-") as [
              "price" | "nameEn" | "createdAt" | "totalSold",
              "asc" | "desc",
            ]
            setParams({ sortBy, sortOrder, page: 1 })
          }}
        >
          <SelectTrigger className="h-10">
            <SelectValue placeholder={messages.product.filters.sortBy} />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="createdAt-desc">{messages.product.filters.sortNewest}</SelectItem>
            <SelectItem value="price-asc">{messages.product.filters.sortPriceAsc}</SelectItem>
            <SelectItem value="price-desc">{messages.product.filters.sortPriceDesc}</SelectItem>
            <SelectItem value="nameEn-asc">{messages.product.filters.sortName}</SelectItem>
            <SelectItem value="totalSold-desc">{messages.product.filters.sortBestSelling}</SelectItem>
          </SelectContent>
        </Select>
      </div>
    </div>
  )
}
