"use client"

import Link from "next/link"
import { Button } from "@/components/ui/button"
import { PageContainer } from "@/components/layout/page-container"
import { CartEmptyState } from "../blocks/cart-empty-state"
import { CartItemRow } from "../blocks/cart-item-row"
import { CartSummary } from "../blocks/cart-summary"
import { useCart } from "../../hooks/useCart"
import { messages } from "@/lib/messages.ar"

export function CartPage() {
  const { data: cart, isLoading } = useCart()

  if (isLoading) {
    return (
      <PageContainer>
        <p className="py-16 text-center">{messages.common.loading}</p>
      </PageContainer>
    )
  }

  if (!cart || cart.items.length === 0) {
    return (
      <PageContainer>
        <h1 className="mb-6 text-2xl font-bold">{messages.cart.title}</h1>
        <CartEmptyState />
      </PageContainer>
    )
  }

  return (
    <PageContainer className="pb-24 lg:pb-6">
      <h1 className="mb-6 text-2xl font-bold">{messages.cart.title}</h1>
      <div className="grid gap-8 lg:grid-cols-3">
        <div className="lg:col-span-2">
          {cart.items.map((item) => (
            <CartItemRow key={item.id} item={item} />
          ))}
          <div className="mt-4 lg:hidden">
            <Button variant="outline" asChild className="h-11 w-full">
              <Link href="/products">{messages.cart.continueShopping}</Link>
            </Button>
          </div>
        </div>
        <div>
          <CartSummary cart={cart} />
        </div>
      </div>
    </PageContainer>
  )
}
