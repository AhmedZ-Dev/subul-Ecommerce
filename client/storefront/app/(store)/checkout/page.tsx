import type { Metadata } from "next"
import { CheckoutPage } from "@/features/checkout"
import { messages } from "@/lib/messages.ar"

export const metadata: Metadata = {
  title: messages.checkout.title,
}

export default function CheckoutRoutePage() {
  return <CheckoutPage />
}
