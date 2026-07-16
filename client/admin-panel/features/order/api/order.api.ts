// features/order/api/order.api.ts
import apiClient from '@/lib/api-client';
import type {
  OrderDto,
  OrderFulfillmentStatus,
  OrderItemDto,
  OrderListItem,
  OrderPaymentStatus,
  OrderQueryParams,
  OrderStatus,
} from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendOrderItem {
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

interface BackendOrderListItem {
  id: number;
  orderNumber: string;
  userId: number | null;
  status: string;
  paymentStatus: string;
  fulfillmentStatus: string;
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

interface BackendOrderDetail extends BackendOrderListItem {
  subtotal: number;
  discountAmount: number;
  shippingAmount: number;
  taxAmount: number;
  couponCode: string | null;
  shippingLastName: string | null;
  shippingAddress1: string | null;
  shippingAddress2: string | null;
  shippingCountry: string;
  shippingZoneId: number | null;
  notes: string | null;
  customerNotes: string | null;
  cancelledAt: string | null;
  cancelReason: string | null;
  shippedAt: string | null;
  deliveredAt: string | null;
  items: BackendOrderItem[];
}

function normalizeStatus(status: string): OrderStatus {
  return status.toLowerCase() as OrderStatus;
}

function normalizePaymentStatus(status: string): OrderPaymentStatus {
  return status.toLowerCase() as OrderPaymentStatus;
}

function normalizeFulfillmentStatus(status: string): OrderFulfillmentStatus {
  return status.toLowerCase() as OrderFulfillmentStatus;
}

function toOrderItem(raw: BackendOrderItem): OrderItemDto {
  return {
    id: raw.id,
    orderId: raw.orderId,
    productId: raw.productId,
    variantId: raw.variantId,
    productName: raw.productName,
    sku: raw.sku,
    quantity: raw.quantity,
    unitPrice: raw.unitPrice,
    compareAtPrice: raw.compareAtPrice,
    discountAmount: raw.discountAmount,
    totalPrice: raw.totalPrice,
    warrantyMonths: raw.warrantyMonths,
    requiresShipping: raw.requiresShipping,
    createdAt: raw.createdAt,
  };
}

function toListItem(raw: BackendOrderListItem): OrderListItem {
  return {
    id: raw.id,
    orderNumber: raw.orderNumber,
    status: normalizeStatus(raw.status),
    paymentStatus: normalizePaymentStatus(raw.paymentStatus),
    fulfillmentStatus: normalizeFulfillmentStatus(raw.fulfillmentStatus),
    total: raw.total,
    currency: raw.currency,
    shippingFirstName: raw.shippingFirstName,
    shippingPhone: raw.shippingPhone,
    shippingCity: raw.shippingCity,
    shippingGovernorate: raw.shippingGovernorate,
    paymentMethod: raw.paymentMethod,
    trackingNumber: raw.trackingNumber,
    createdAt: raw.createdAt,
    updatedAt: raw.updatedAt,
    itemCount: raw.itemCount,
  };
}

function toDto(raw: BackendOrderDetail): OrderDto {
  return {
    id: raw.id,
    orderNumber: raw.orderNumber,
    userId: raw.userId,
    status: normalizeStatus(raw.status),
    paymentStatus: normalizePaymentStatus(raw.paymentStatus),
    fulfillmentStatus: normalizeFulfillmentStatus(raw.fulfillmentStatus),
    subtotal: raw.subtotal,
    discountAmount: raw.discountAmount,
    shippingAmount: raw.shippingAmount,
    taxAmount: raw.taxAmount,
    total: raw.total,
    currency: raw.currency,
    couponCode: raw.couponCode,
    shippingFirstName: raw.shippingFirstName,
    shippingLastName: raw.shippingLastName,
    shippingPhone: raw.shippingPhone,
    shippingAddress1: raw.shippingAddress1,
    shippingAddress2: raw.shippingAddress2,
    shippingCity: raw.shippingCity,
    shippingGovernorate: raw.shippingGovernorate,
    shippingCountry: raw.shippingCountry,
    shippingZoneId: raw.shippingZoneId,
    paymentMethod: raw.paymentMethod,
    trackingNumber: raw.trackingNumber,
    notes: raw.notes,
    customerNotes: raw.customerNotes,
    cancelledAt: raw.cancelledAt,
    cancelReason: raw.cancelReason,
    shippedAt: raw.shippedAt,
    deliveredAt: raw.deliveredAt,
    createdAt: raw.createdAt,
    updatedAt: raw.updatedAt,
    items: (raw.items ?? []).map(toOrderItem),
  };
}

export interface UpdateOrderPayload {
  status?: OrderStatus;
  paymentStatus?: OrderPaymentStatus;
  fulfillmentStatus?: OrderFulfillmentStatus;
  trackingNumber?: string | null;
  notes?: string | null;
  cancelReason?: string | null;
}

function buildUpdateBody(payload: UpdateOrderPayload) {
  const body: Record<string, string | null> = {};

  if (payload.status !== undefined) body.status = payload.status;
  if (payload.paymentStatus !== undefined) body.paymentStatus = payload.paymentStatus;
  if (payload.fulfillmentStatus !== undefined) body.fulfillmentStatus = payload.fulfillmentStatus;
  if (payload.trackingNumber !== undefined) {
    body.trackingNumber = payload.trackingNumber?.trim() || null;
  }
  if (payload.notes !== undefined) {
    body.notes = payload.notes?.trim() || null;
  }
  if (payload.cancelReason !== undefined) {
    body.cancelReason = payload.cancelReason?.trim() || null;
  }

  return body;
}

export async function getOrders(
  params: OrderQueryParams = {},
): Promise<PaginatedResponse<OrderListItem>> {
  const {
    page = 1,
    limit = 10,
    search,
    status,
    paymentStatus,
    fulfillmentStatus,
    sortBy,
    sortOrder,
  } = params;

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendOrderListItem[];
      total: number;
      page: number;
      limit: number;
      totalPages: number;
    }>
  >('/orders', {
    params: {
      page,
      limit,
      ...(search && { search }),
      ...(status && { status }),
      ...(paymentStatus && { paymentStatus }),
      ...(fulfillmentStatus && { fulfillmentStatus }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch orders');

  const raw = data.data!;
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}

export async function getOrderById(id: number): Promise<OrderDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendOrderDetail>>(`/orders/${id}`);
  if (!data.success) return null;
  return data.data ? toDto(data.data) : null;
}

export async function updateOrder(id: number, payload: UpdateOrderPayload): Promise<OrderDto> {
  const { data } = await apiClient.put<ApiResponse<BackendOrderDetail>>(
    `/orders/${id}`,
    buildUpdateBody(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to update order');
  return toDto(data.data!);
}
