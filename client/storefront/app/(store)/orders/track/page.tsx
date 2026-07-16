import type { Metadata } from "next"
import { OrderTrackPage } from "@/features/order"
import { messages } from "@/lib/messages.ar"

export const metadata: Metadata = {
  title: messages.order.trackTitle,
}

export default function OrderTrackRoutePage() {
  return <OrderTrackPage />
}
