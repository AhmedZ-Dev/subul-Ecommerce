"use client"

import { useEffect } from "react"
import { useRouter } from "next/navigation"
import { PageContainer } from "@/components/layout/page-container"
import { CheckoutForm } from "../blocks/checkout-form"
import { OrderSummary } from "../blocks/order-summary"
import { useCart } from "@/features/cart"
import { useCreateOrder } from "../../hooks/useCheckout"
import { formatCurrency, messages } from "@/lib/messages.ar"

export function CheckoutPage() {
  const router = useRouter()
  const { data: cart, isLoading } = useCart()
  const createOrder = useCreateOrder()

  useEffect(() => {
    // Don't bounce to cart while submitting / right after success (cart is cleared first)
    if (createOrder.isPending || createOrder.isSuccess) return
    if (!isLoading && (!cart || cart.items.length === 0)) {
      router.replace("/cart")
    }
  }, [cart, isLoading, router, createOrder.isPending, createOrder.isSuccess])

  if (isLoading) {
    return (
      <PageContainer>
        <p className="py-16 text-center">{messages.common.loading}</p>
      </PageContainer>
    )
  }

  if (!cart || cart.items.length === 0) {
    return null
  }

  return (
    <PageContainer>
      <div className="mb-6 md:mb-8">
        <h1 className="text-2xl font-bold tracking-tight md:text-3xl">
          {messages.checkout.title}
        </h1>
        <p className="text-muted-foreground mt-1.5 text-sm md:text-base">
          {messages.checkout.description}
        </p>
      </div>

      <details className="mb-6 lg:hidden">
        <summary className="bg-card hover:bg-muted/50 flex h-11 cursor-pointer list-none items-center justify-between rounded-lg border px-4 text-sm font-medium [&::-webkit-details-marker]:hidden">
          <span>{messages.checkout.orderSummary}</span>
          <span className="text-primary font-semibold">
            {messages.checkout.total}: {formatCurrency(cart.subtotal)}
          </span>
        </summary>
        <div className="mt-3">
          <OrderSummary cart={cart} />
        </div>
      </details>

      <div className="grid gap-8 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <CheckoutForm
            onSubmit={(values) => createOrder.mutate(values)}
            isSubmitting={createOrder.isPending}
          />
        </div>
        <div className="hidden lg:block">
          <OrderSummary cart={cart} />
        </div>
      </div>
    </PageContainer>
  )
}
