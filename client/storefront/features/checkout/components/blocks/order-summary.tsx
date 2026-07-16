import { Card, CardContent } from "@/components/ui/card"
import { formatCurrency, getProductName, messages } from "@/lib/messages.ar"
import type { Cart } from "@/features/cart"

interface OrderSummaryProps {
  cart: Cart
  compact?: boolean
}

export function OrderSummary({ cart, compact = false }: OrderSummaryProps) {
  return (
    <Card>
      <CardContent className="flex flex-col gap-4 p-4 md:p-6">
        <h2 className="text-lg font-semibold">{messages.checkout.orderSummary}</h2>
        {!compact && (
          <div className="flex flex-col gap-3">
            {cart.items.map((item) => (
              <div key={item.id} className="flex justify-between gap-2 text-sm">
                <span className="line-clamp-2 min-w-0">
                  {getProductName(item.productNameAr, item.productNameEn)} × {item.quantity}
                </span>
                <span className="shrink-0">{formatCurrency(item.lineTotal)}</span>
              </div>
            ))}
          </div>
        )}
        {compact && (
          <p className="text-muted-foreground text-sm">
            {messages.cart.itemCount(cart.itemCount)}
          </p>
        )}
        <div className="border-t pt-4">
          <div className="flex justify-between font-semibold">
            <span>{messages.checkout.subtotal}</span>
            <span>{formatCurrency(cart.subtotal)}</span>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
