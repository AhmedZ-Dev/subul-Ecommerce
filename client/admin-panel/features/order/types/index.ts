// features/order/types/index.ts

export const ORDER_STATUSES = [
  'pending',
  'confirmed',
  'processing',
  'shipped',
  'out_for_delivery',
  'delivered',
  'cancelled',
  'refunded',
] as const;

export const ORDER_PAYMENT_STATUSES = ['pending', 'paid', 'refunded'] as const;

export const ORDER_FULFILLMENT_STATUSES = ['unfulfilled', 'partial', 'fulfilled'] as const;

export type OrderStatus = (typeof ORDER_STATUSES)[number];
export type OrderPaymentStatus = (typeof ORDER_PAYMENT_STATUSES)[number];
export type OrderFulfillmentStatus = (typeof ORDER_FULFILLMENT_STATUSES)[number];

export interface OrderItemDto {
  id: number;
  orderId: number;
  productId: number | null;
  variantId: number | null;
  productName: string;
  sku: string | null;
  quantity: number;
  unitPrice: number;
  compareAtPrice: number | null;
  discountAmount: number;
  totalPrice: number;
  warrantyMonths: number;
  requiresShipping: boolean;
  createdAt: string;
}

export interface OrderDto {
  id: number;
  orderNumber: string;
  userId: number | null;
  status: OrderStatus;
  paymentStatus: OrderPaymentStatus;
  fulfillmentStatus: OrderFulfillmentStatus;
  subtotal: number;
  discountAmount: number;
  shippingAmount: number;
  taxAmount: number;
  total: number;
  currency: string;
  couponCode: string | null;
  shippingFirstName: string | null;
  shippingLastName: string | null;
  shippingPhone: string | null;
  shippingAddress1: string | null;
  shippingAddress2: string | null;
  shippingCity: string | null;
  shippingGovernorate: string | null;
  shippingCountry: string;
  shippingZoneId: number | null;
  paymentMethod: string | null;
  trackingNumber: string | null;
  notes: string | null;
  customerNotes: string | null;
  cancelledAt: string | null;
  cancelReason: string | null;
  shippedAt: string | null;
  deliveredAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  items: OrderItemDto[];
}

export interface OrderListItem {
  id: number;
  orderNumber: string;
  status: OrderStatus;
  paymentStatus: OrderPaymentStatus;
  fulfillmentStatus: OrderFulfillmentStatus;
  total: number;
  currency: string;
  shippingFirstName: string | null;
  shippingPhone: string | null;
  shippingCity: string | null;
  shippingGovernorate: string | null;
  paymentMethod: string | null;
  trackingNumber: string | null;
  createdAt: string;
  updatedAt: string | null;
  itemCount: number;
}

export interface OrderQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  status?: OrderStatus;
  paymentStatus?: OrderPaymentStatus;
  fulfillmentStatus?: OrderFulfillmentStatus;
  sortBy?: 'orderNumber' | 'total' | 'status' | 'createdAt' | 'updatedAt';
  sortOrder?: 'asc' | 'desc';
}
