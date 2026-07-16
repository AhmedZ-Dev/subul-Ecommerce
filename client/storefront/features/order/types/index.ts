export interface GuestOrderTrackItem {
  productName: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

export interface GuestOrderTrackResult {
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
  items: GuestOrderTrackItem[]
}
