"use client"

import { useState } from "react"
import { ShoppingCart } from "lucide-react"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { useAddToCart } from "@/features/cart"
import { messages } from "@/lib/messages.ar"

interface AddToCartButtonProps {
  productId: number
  variantId?: number
  quantity?: number
  disabled?: boolean
  className?: string
  showIcon?: boolean
}

export function AddToCartButton({
  productId,
  variantId,
  quantity = 1,
  disabled,
  className,
  showIcon = false,
}: AddToCartButtonProps) {
  const addToCart = useAddToCart()
  const [loading, setLoading] = useState(false)

  async function handleClick() {
    setLoading(true)
    try {
      await addToCart.mutateAsync({ productId, variantId, quantity })
      toast.success(messages.cart.addSuccess)
    } catch (err) {
      toast.error(err instanceof Error ? err.message : messages.cart.addError)
    } finally {
      setLoading(false)
    }
  }

  return (
    <Button
      className={className}
      disabled={disabled || loading}
      onClick={handleClick}
    >
      {showIcon && !loading && <ShoppingCart className="size-4 shrink-0" />}
      {loading ? messages.common.loading : messages.product.addToCart}
    </Button>
  )
}
