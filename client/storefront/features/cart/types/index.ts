export interface CartItem {
  id: number
  productId: number
  variantId: number | null
  productNameEn: string
  productNameAr: string | null
  productSlug: string
  sku: string | null
  quantity: number
  unitPrice: number
  lineTotal: number
}

export interface Cart {
  id: number
  sessionId: string
  userId: number | null
  couponCode: string | null
  notes: string | null
  items: CartItem[]
  subtotal: number
  itemCount: number
}

export interface AddToCartPayload {
  productId: number
  variantId?: number
  quantity?: number
}

export interface UpdateCartItemPayload {
  quantity: number
}
