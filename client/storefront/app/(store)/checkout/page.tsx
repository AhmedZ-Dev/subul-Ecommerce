import type { Metadata } from "next"
import { CheckoutPage } from "@/features/checkout"
import { messages } from "@/lib/messages.ar"

export const metadata: Metadata = {
  title: messages.checkout.title,
  // Per-visitor page — nothing here belongs in a search index.
  robots: { index: false, follow: true },
}

export default function CheckoutRoutePage() {
  return <CheckoutPage />
}
