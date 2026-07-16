import { z } from "zod"
import { messages } from "@/lib/messages.ar"

export const checkoutSchema = z.object({
  firstName: z.string().min(2, messages.checkout.validation.firstName),
  lastName: z.string().min(2, messages.checkout.validation.lastName),
  phone: z.string().min(7, messages.checkout.validation.phone),
  address1: z.string().min(5, messages.checkout.validation.address),
  address2: z.string().optional(),
  city: z.string().min(2, messages.checkout.validation.city),
  governorate: z.string().min(2, messages.checkout.validation.governorate),
  shippingZoneId: z.number().optional(),
  paymentMethod: z.literal("cod"),
  customerNotes: z.string().max(500).optional(),
})

export type CheckoutFormValues = z.infer<typeof checkoutSchema>
