"use client"

import Link from "next/link"
import { ShoppingCart } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { useCartCount } from "@/features/cart"
import { messages } from "@/lib/messages.ar"

export function CartButton() {
  const count = useCartCount()

  return (
    <Button variant="outline" size="icon" className="relative size-11 shrink-0" asChild>
      <Link href="/cart" aria-label={messages.header.cart}>
        <ShoppingCart className="size-5" />
        {count > 0 && (
          <Badge
            variant="default"
            className="absolute -top-2 -end-2 size-5 justify-center rounded-full p-0 text-xs"
          >
            {count > 99 ? "99+" : count}
          </Badge>
        )}
      </Link>
    </Button>
  )
}
