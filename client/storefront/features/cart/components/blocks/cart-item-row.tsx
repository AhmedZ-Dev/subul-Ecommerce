"use client"

import Link from "next/link"
import { Minus, Package, Plus, Trash2 } from "lucide-react"
import { Button } from "@/components/ui/button"
import { getProductName, formatCurrency, messages } from "@/lib/messages.ar"
import { useRemoveCartItem, useUpdateCartItem } from "../../hooks/useCartMutations"
import type { CartItem } from "../../types"

interface CartItemRowProps {
  item: CartItem
}

export function CartItemRow({ item }: CartItemRowProps) {
  const updateItem = useUpdateCartItem()
  const removeItem = useRemoveCartItem()
  const name = getProductName(item.productNameAr, item.productNameEn)

  return (
    <div className="flex flex-col gap-4 border-b py-4 sm:flex-row sm:items-center">
      <div className="flex min-w-0 flex-1 items-start gap-3">
        <div className="bg-muted relative flex size-16 shrink-0 items-center justify-center overflow-hidden rounded-lg">
          <div className="hero-gradient absolute inset-0 opacity-30" aria-hidden />
          <Package className="text-muted-foreground relative size-6" aria-hidden />
        </div>
        <div className="flex min-w-0 flex-1 flex-col gap-1">
          <Link
            href={`/products/${item.productSlug}`}
            className="hover:text-primary line-clamp-2 font-medium transition-colors"
          >
            {name}
          </Link>
          {item.sku && (
            <p className="text-muted-foreground text-xs">{item.sku}</p>
          )}
          <p className="text-primary text-sm font-semibold">
            {formatCurrency(item.unitPrice)}
          </p>
        </div>
      </div>

      <div className="flex items-center justify-between gap-3 sm:justify-end">
        <div className="flex items-center gap-2">
          <Button
            type="button"
            variant="outline"
            size="icon"
            className="size-11"
            disabled={item.quantity <= 1 || updateItem.isPending}
            onClick={() =>
              updateItem.mutate({ itemId: item.id, payload: { quantity: item.quantity - 1 } })
            }
            aria-label={messages.cart.quantity}
          >
            <Minus className="size-4" />
          </Button>
          <span className="w-8 text-center text-sm font-medium">{item.quantity}</span>
          <Button
            type="button"
            variant="outline"
            size="icon"
            className="size-11"
            disabled={updateItem.isPending}
            onClick={() =>
              updateItem.mutate({ itemId: item.id, payload: { quantity: item.quantity + 1 } })
            }
            aria-label={messages.cart.quantity}
          >
            <Plus className="size-4" />
          </Button>
        </div>

        <div className="flex items-center gap-3">
          <p className="text-primary font-semibold">
            {formatCurrency(item.lineTotal)}
          </p>
          <Button
            type="button"
            variant="ghost"
            size="icon"
            className="text-destructive size-11"
            disabled={removeItem.isPending}
            onClick={() => removeItem.mutate(item.id)}
            aria-label={messages.cart.remove}
          >
            <Trash2 className="size-4" />
          </Button>
        </div>
      </div>
    </div>
  )
}
