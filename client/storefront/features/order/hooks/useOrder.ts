"use client"

import { useMutation } from "@tanstack/react-query"
import { toast } from "sonner"
import { trackGuestOrder } from "../api/order.api"
import { messages } from "@/lib/messages.ar"

export function useTrackOrder() {
  return useMutation({
    mutationFn: ({ orderNumber, phone }: { orderNumber: string; phone: string }) =>
      trackGuestOrder(orderNumber, phone),
    onError: (error: Error) => {
      toast.error(error.message ?? messages.order.trackError)
    },
  })
}
