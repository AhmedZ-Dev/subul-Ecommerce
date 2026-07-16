// features/order/constants/index.ts

import type { StatusTone } from '@/components/ui/status-indicator';
import type {
  OrderFulfillmentStatus,
  OrderPaymentStatus,
  OrderStatus,
} from '../types';

export const ORDER_QUERY_KEYS = {
  ALL: ['orders'] as const,
};

export const ORDER_DEFAULT_PAGE_SIZE = 10;
export const ORDER_MAX_PAGE_SIZE = 100;

export const ORDER_STATUS_TONES: Record<OrderStatus, StatusTone> = {
  pending: 'warning',
  confirmed: 'info',
  processing: 'info',
  shipped: 'info',
  out_for_delivery: 'info',
  delivered: 'success',
  cancelled: 'neutral',
  refunded: 'neutral',
};

export const ORDER_STATUS_COLORS: Record<OrderStatus, string> = {
  pending:
    'bg-amber-50 text-amber-700 border-amber-200 dark:bg-amber-950/40 dark:text-amber-400 dark:border-amber-900',
  confirmed:
    'bg-blue-50 text-blue-700 border-blue-200 dark:bg-blue-950/40 dark:text-blue-400 dark:border-blue-900',
  processing:
    'bg-indigo-50 text-indigo-700 border-indigo-200 dark:bg-indigo-950/40 dark:text-indigo-400 dark:border-indigo-900',
  shipped:
    'bg-sky-50 text-sky-700 border-sky-200 dark:bg-sky-950/40 dark:text-sky-400 dark:border-sky-900',
  out_for_delivery:
    'bg-cyan-50 text-cyan-700 border-cyan-200 dark:bg-cyan-950/40 dark:text-cyan-400 dark:border-cyan-900',
  delivered:
    'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  cancelled:
    'bg-gray-50 text-gray-700 border-gray-200 dark:bg-gray-900/40 dark:text-gray-400 dark:border-gray-800',
  refunded:
    'bg-purple-50 text-purple-700 border-purple-200 dark:bg-purple-950/40 dark:text-purple-400 dark:border-purple-900',
};

export const ORDER_STATUS_DOT_COLORS: Record<OrderStatus, string> = {
  pending: 'bg-amber-500',
  confirmed: 'bg-blue-500',
  processing: 'bg-indigo-500',
  shipped: 'bg-sky-500',
  out_for_delivery: 'bg-cyan-500',
  delivered: 'bg-green-500',
  cancelled: 'bg-muted-foreground/60',
  refunded: 'bg-purple-500',
};

export const ORDER_PAYMENT_STATUS_TONES: Record<OrderPaymentStatus, StatusTone> = {
  pending: 'warning',
  paid: 'success',
  refunded: 'neutral',
};

export const ORDER_PAYMENT_STATUS_COLORS: Record<OrderPaymentStatus, string> = {
  pending:
    'bg-amber-50 text-amber-700 border-amber-200 dark:bg-amber-950/40 dark:text-amber-400 dark:border-amber-900',
  paid: 'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  refunded:
    'bg-purple-50 text-purple-700 border-purple-200 dark:bg-purple-950/40 dark:text-purple-400 dark:border-purple-900',
};

export const ORDER_PAYMENT_STATUS_DOT_COLORS: Record<OrderPaymentStatus, string> = {
  pending: 'bg-amber-500',
  paid: 'bg-green-500',
  refunded: 'bg-purple-500',
};

export const ORDER_FULFILLMENT_STATUS_TONES: Record<OrderFulfillmentStatus, StatusTone> = {
  unfulfilled: 'warning',
  partial: 'info',
  fulfilled: 'success',
};

export const ORDER_FULFILLMENT_STATUS_COLORS: Record<OrderFulfillmentStatus, string> = {
  unfulfilled:
    'bg-amber-50 text-amber-700 border-amber-200 dark:bg-amber-950/40 dark:text-amber-400 dark:border-amber-900',
  partial:
    'bg-blue-50 text-blue-700 border-blue-200 dark:bg-blue-950/40 dark:text-blue-400 dark:border-blue-900',
  fulfilled:
    'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
};

export const ORDER_FULFILLMENT_STATUS_DOT_COLORS: Record<OrderFulfillmentStatus, string> = {
  unfulfilled: 'bg-amber-500',
  partial: 'bg-blue-500',
  fulfilled: 'bg-green-500',
};
