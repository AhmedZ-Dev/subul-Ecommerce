import type { Metadata } from "next"
import { CartPage } from "@/features/cart"
import { messages } from "@/lib/messages.ar"

export const metadata: Metadata = {
  title: messages.cart.title,
}

export default function CartRoutePage() {
  return <CartPage />
}
