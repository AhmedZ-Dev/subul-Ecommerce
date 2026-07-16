// features/order/index.ts
// Public barrel — the ONLY way to import from the order feature.

// ── Components ────────────────────────────────────────────────────────────────
export { OrderListingPage } from './components/pages/order-listing-page';
export { OrderView } from './components/pages/order-view';

// ── Hooks ─────────────────────────────────────────────────────────────────────
export { useOrders, useOrder, orderKeys } from './hooks/useOrder';
export { useUpdateOrder } from './hooks/useOrderMutations';

// ── Types ─────────────────────────────────────────────────────────────────────
export type {
  OrderDto,
  OrderItemDto,
  OrderListItem,
  OrderQueryParams,
  OrderStatus,
  OrderPaymentStatus,
  OrderFulfillmentStatus,
} from './types';

// ── Schemas + inferred types ──────────────────────────────────────────────────
export { updateOrderSchema } from './schemas/order.schema';
export type { UpdateOrderInput } from './schemas/order.schema';

// ── API (for RSC server-side calls) ──────────────────────────────────────────
export { getOrders, getOrderById, updateOrder } from './api/order.api';
export type { UpdateOrderPayload } from './api/order.api';
export { getCachedOrderById } from './api/order.cached';

// ── URL search params (nuqs) ────────────────────────────────────────────────────
export { orderListingParsers, normalizeOrderPageSize } from './search-params';
