import apiClient from "@/lib/api-client"
import {
  clearCartSessionId,
  getCartSessionId,
  setCartSessionId,
} from "@/lib/cart-session"
import type { ApiResponse } from "@/types/api"
import type {
  AddToCartPayload,
  Cart,
  CartItem,
  UpdateCartItemPayload,
} from "../types"

interface BackendCartItem {
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

interface BackendCart {
  id: number
  sessionId: string
  userId: number | null
  couponCode: string | null
  notes: string | null
  items: BackendCartItem[]
  subtotal: number
  itemCount: number
}

function toCartItem(raw: BackendCartItem): CartItem {
  return {
    id: raw.id,
    productId: raw.productId,
    variantId: raw.variantId,
    productNameEn: raw.productNameEn,
    productNameAr: raw.productNameAr,
    productSlug: raw.productSlug,
    sku: raw.sku,
    quantity: raw.quantity,
    unitPrice: raw.unitPrice,
    lineTotal: raw.lineTotal,
  }
}

function toCart(raw: BackendCart): Cart {
  return {
    id: raw.id,
    sessionId: raw.sessionId,
    userId: raw.userId,
    couponCode: raw.couponCode,
    notes: raw.notes,
    items: raw.items.map(toCartItem),
    subtotal: raw.subtotal,
    itemCount: raw.itemCount,
  }
}

function cartHeaders(): Record<string, string> {
  const sessionId = getCartSessionId()
  return sessionId ? { "X-Cart-Session": sessionId } : {}
}

export async function getCart(): Promise<Cart | null> {
  const sessionId = getCartSessionId()
  if (!sessionId) return null

  const { data } = await apiClient.get<ApiResponse<BackendCart>>("/carts", {
    headers: cartHeaders(),
  })

  if (!data.success) return null
  return data.data ? toCart(data.data) : null
}

export async function addCartItem(payload: AddToCartPayload): Promise<Cart> {
  const response = await apiClient.post<ApiResponse<{ cart: BackendCart; sessionId: string }>>(
    "/carts/items",
    {
      productId: payload.productId,
      variantId: payload.variantId ?? null,
      quantity: payload.quantity ?? 1,
    },
    { headers: cartHeaders() },
  )

  const { data } = response
  if (!data.success) throw new Error(data.message ?? "Failed to add to cart")

  const newSessionId =
    response.headers["x-cart-session"] ?? data.data?.sessionId
  if (newSessionId) setCartSessionId(newSessionId)

  return toCart(data.data!.cart)
}

export async function updateCartItem(
  itemId: number,
  payload: UpdateCartItemPayload,
): Promise<Cart> {
  const sessionId = getCartSessionId()
  if (!sessionId) throw new Error("Cart session is required")

  const { data } = await apiClient.put<ApiResponse<BackendCart>>(
    `/carts/items/${itemId}`,
    { quantity: payload.quantity },
    { headers: cartHeaders() },
  )

  if (!data.success) throw new Error(data.message ?? "Failed to update cart item")
  return toCart(data.data!)
}

export async function removeCartItem(itemId: number): Promise<void> {
  const sessionId = getCartSessionId()
  if (!sessionId) throw new Error("Cart session is required")

  const { data } = await apiClient.delete<ApiResponse<boolean>>(
    `/carts/items/${itemId}`,
    { headers: cartHeaders() },
  )

  if (!data.success) throw new Error(data.message ?? "Failed to remove cart item")
}

export function clearCart(): void {
  clearCartSessionId()
}
