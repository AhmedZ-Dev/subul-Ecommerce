import apiClient from "@/lib/api-client"
import type { ApiResponse } from "@/types/api"
import type { GuestOrderTrackResult } from "../types"

interface BackendTrackOrder {
  id: number
  orderNumber: string
  status: string
  paymentStatus: string
  fulfillmentStatus: string
  total: number
  currency: string
  shippingCity: string | null
  shippingGovernorate: string | null
  trackingNumber: string | null
  shippedAt: string | null
  deliveredAt: string | null
  createdAt: string
  items: {
    productName: string
    quantity: number
    unitPrice: number
    totalPrice: number
  }[]
}

function toResult(raw: BackendTrackOrder): GuestOrderTrackResult {
  return {
    id: raw.id,
    orderNumber: raw.orderNumber,
    status: raw.status,
    paymentStatus: raw.paymentStatus,
    fulfillmentStatus: raw.fulfillmentStatus,
    total: raw.total,
    currency: raw.currency,
    shippingCity: raw.shippingCity,
    shippingGovernorate: raw.shippingGovernorate,
    trackingNumber: raw.trackingNumber,
    shippedAt: raw.shippedAt,
    deliveredAt: raw.deliveredAt,
    createdAt: raw.createdAt,
    items: raw.items,
  }
}

export async function trackGuestOrder(
  orderNumber: string,
  phone: string,
): Promise<GuestOrderTrackResult | null> {
  const { data } = await apiClient.get<ApiResponse<BackendTrackOrder>>("/orders/track", {
    params: { orderNumber, phone },
  })

  if (!data.success) return null
  return data.data ? toResult(data.data) : null
}
