import { z } from "zod"
import { messages } from "@/lib/messages.ar"

export const orderTrackSchema = z.object({
  orderNumber: z.string().min(1, messages.order.orderNumber),
  phone: z.string().min(7, messages.order.phone),
})

export type OrderTrackFormValues = z.infer<typeof orderTrackSchema>
