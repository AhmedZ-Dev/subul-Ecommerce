import Link from "next/link"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { formatCurrency, messages } from "@/lib/messages.ar"
import type { Cart } from "../../types"

interface CartSummaryProps {
  cart: Cart
}

export function CartSummary({ cart }: CartSummaryProps) {
  return (
    <>
      <Card className="hidden lg:sticky lg:top-[calc(var(--header-height)+1rem)] lg:block">
        <CardContent className="flex flex-col gap-4 p-6">
          <h2 className="text-lg font-semibold">{messages.checkout.orderSummary}</h2>
          <div className="flex justify-between text-sm">
            <span className="text-muted-foreground">{messages.cart.subtotal}</span>
            <span className="font-semibold">{formatCurrency(cart.subtotal)}</span>
          </div>
          <div className="flex justify-between text-sm">
            <span className="text-muted-foreground">{messages.cart.itemCount(cart.itemCount)}</span>
          </div>
          <div className="border-t pt-4">
            <div className="flex justify-between font-bold">
              <span>{messages.checkout.total}</span>
              <span className="text-primary">{formatCurrency(cart.subtotal)}</span>
            </div>
          </div>
          <Button asChild className="w-full" size="lg">
            <Link href="/checkout">{messages.cart.checkout}</Link>
          </Button>
          <Button variant="outline" asChild className="w-full">
            <Link href="/products">{messages.cart.continueShopping}</Link>
          </Button>
        </CardContent>
      </Card>

      <div className="mobile-sticky-bar lg:hidden">
        <div className="container mx-auto flex items-center gap-3 px-4 py-3">
          <div className="min-w-0 flex-1">
            <p className="text-muted-foreground text-xs">{messages.checkout.total}</p>
            <p className="text-primary text-lg font-bold">{formatCurrency(cart.subtotal)}</p>
          </div>
          <Button asChild size="lg" className="h-11 shrink-0 px-6">
            <Link href="/checkout">{messages.cart.checkout}</Link>
          </Button>
        </div>
      </div>
    </>
  )
}
