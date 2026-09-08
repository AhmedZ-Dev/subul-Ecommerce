import apiClient from "@/lib/api-client"
import { getCartSessionId } from "@/lib/cart-session"
import { messages } from "@/lib/messages.ar"
import type { ApiResponse } from "@/types/api"
import type { CheckoutFormValues } from "../schemas/checkout.schema"

export interface OrderConfirmation {
  id: number
  orderNumber: string
  status: string
  paymentStatus: string
  fulfillmentStatus: string
  subtotal: number
  discountAmount: number
  shippingAmount: number
  taxAmount: number
  total: number
  currency: string
  shippingFirstName: string | null
  shippingPhone: string | null
  shippingCity: string | null
  shippingGovernorate: string | null
  paymentMethod: string | null
  createdAt: string
  items: {
    id: number
    productId: number | null
    variantId: number | null
    productName: string
    sku: string | null
    quantity: number
    unitPrice: number
    totalPrice: number
  }[]
}

interface BackendOrderResponse {
  id: number
  orderNumber: string
  status: string
  paymentStatus: string
  fulfillmentStatus: string
  subtotal: number
  discountAmount: number
  shippingAmount: number
  taxAmount: number
  total: number
  currency: string
  shippingFirstName: string | null
  shippingPhone: string | null
  shippingCity: string | null
  shippingGovernorate: string | null
  paymentMethod: string | null
  createdAt: string
  items: {
    id: number
    productId: number | null
    variantId: number | null
    productName: string
    sku: string | null
    quantity: number
    unitPrice: number
    totalPrice: number
  }[]
}

function toConfirmation(raw: BackendOrderResponse): OrderConfirmation {
  return {
    id: raw.id,
    orderNumber: raw.orderNumber,
    status: raw.status,
    paymentStatus: raw.paymentStatus,
    fulfillmentStatus: raw.fulfillmentStatus,
    subtotal: raw.subtotal,
    discountAmount: raw.discountAmount,
    shippingAmount: raw.shippingAmount,
    taxAmount: raw.taxAmount,
    total: raw.total,
    currency: raw.currency,
    shippingFirstName: raw.shippingFirstName,
    shippingPhone: raw.shippingPhone,
    shippingCity: raw.shippingCity,
    shippingGovernorate: raw.shippingGovernorate,
    paymentMethod: raw.paymentMethod,
    createdAt: raw.createdAt,
    items: raw.items,
  }
}

export async function createOrder(payload: CheckoutFormValues): Promise<OrderConfirmation> {
  const sessionId = getCartSessionId()
  if (!sessionId) throw new Error("Cart session is required")

  const { data } = await apiClient.post<ApiResponse<BackendOrderResponse>>(
    "/orders",
    {
      shippingFirstName: payload.firstName,
      shippingLastName: payload.lastName,
      shippingPhone: payload.phone,
      shippingAddress1: payload.address1,
      shippingAddress2: payload.address2 || null,
      shippingCity: payload.city,
      shippingGovernorate: payload.governorate,
      shippingCountry: "Iraq",
      shippingZoneId: payload.shippingZoneId ?? null,
      paymentMethod: payload.paymentMethod,
      customerNotes: payload.customerNotes || null,
    },
    { headers: { "X-Cart-Session": sessionId } },
  )

  // Prefer errors[0]: a validation failure puts the field-specific Arabic
  // message there, while `message` is only the generic "Validation failed".
  if (!data.success) {
    throw new Error(data.errors?.[0] ?? data.message ?? messages.checkout.createError)
  }
  return toConfirmation(data.data!)
}
