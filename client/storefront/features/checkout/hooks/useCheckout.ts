"use client"

import { useMutation } from "@tanstack/react-query"
import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { createOrder } from "../api/checkout.api"
import { useClearCart } from "@/features/cart"
import type { CheckoutFormValues } from "../schemas/checkout.schema"
import { messages } from "@/lib/messages.ar"

export function useCreateOrder() {
  const router = useRouter()
  const clearCart = useClearCart()

  return useMutation({
    mutationFn: (payload: CheckoutFormValues) => createOrder(payload),
    onSuccess: (order) => {
      clearCart()
      toast.success(messages.checkout.success)
      router.replace(
        `/order-confirmation?orderNumber=${encodeURIComponent(order.orderNumber)}`,
      )
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.checkout.createError)
    },
  })
}
