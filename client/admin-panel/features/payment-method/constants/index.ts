// features/payment-method/constants/index.ts

import type { StatusTone } from '@/components/ui/status-indicator';
import type { PaymentMethodStatus, PaymentMethodType } from '../types';

export const PAYMENT_METHOD_QUERY_KEYS = {
  ALL: ['payment-methods'] as const,
};

export const PAYMENT_METHOD_DEFAULT_PAGE_SIZE = 10;
export const PAYMENT_METHOD_MAX_PAGE_SIZE = 100;

export const PAYMENT_METHOD_STATUS_TONES: Record<PaymentMethodStatus, StatusTone> = {
  active: 'success',
  inactive: 'neutral',
};

export const PAYMENT_METHOD_STATUS_COLORS: Record<PaymentMethodStatus, string> = {
  active:
    'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  inactive:
    'bg-gray-50 text-gray-700 border-gray-200 dark:bg-gray-900/40 dark:text-gray-400 dark:border-gray-800',
};

export const PAYMENT_METHOD_STATUS_DOT_COLORS: Record<PaymentMethodStatus, string> = {
  active: 'bg-green-500',
  inactive: 'bg-muted-foreground/60',
};

export const PAYMENT_METHOD_TYPE_OPTIONS: {
  value: PaymentMethodType;
  labelKey: 'offline' | 'online';
}[] = [
  { value: 'offline', labelKey: 'offline' },
  { value: 'online', labelKey: 'online' },
];
