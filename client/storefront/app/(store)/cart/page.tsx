import type { Metadata } from "next"
import { CartPage } from "@/features/cart"
import { messages } from "@/lib/messages.ar"

export const metadata: Metadata = {
  title: messages.cart.title,
  // Per-visitor page — nothing here belongs in a search index.
  robots: { index: false, follow: true },
}

export default function CartRoutePage() {
  return <CartPage />
}
