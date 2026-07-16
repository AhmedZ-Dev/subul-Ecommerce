// features/product/constants/index.ts

import type { StatusTone } from '@/components/ui/status-indicator';
import type { ProductStatus } from '../types';

export const PRODUCT_QUERY_KEYS = {
  ALL: ['products'] as const,
};

export const PRODUCT_VARIANT_QUERY_KEYS = {
  ALL: ['product-variants'] as const,
};

export const PRODUCT_IMAGE_QUERY_KEYS = {
  ALL: ['product-images'] as const,
};

export const PRODUCT_ATTR_VALUE_QUERY_KEYS = {
  ALL: ['product-attribute-values'] as const,
};

export const PRODUCT_DEFAULT_PAGE_SIZE = 10;
export const PRODUCT_MAX_PAGE_SIZE = 100;

export const PRODUCT_STATUS_TONES: Record<ProductStatus, StatusTone> = {
  active: 'success',
  draft: 'warning',
  archived: 'neutral',
};

export const PRODUCT_STATUS_COLORS: Record<ProductStatus, string> = {
  active:
    'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  draft:
    'bg-amber-50 text-amber-700 border-amber-200 dark:bg-amber-950/40 dark:text-amber-400 dark:border-amber-900',
  archived:
    'bg-gray-50 text-gray-700 border-gray-200 dark:bg-gray-900/40 dark:text-gray-400 dark:border-gray-800',
};

export const PRODUCT_STATUS_DOT_COLORS: Record<ProductStatus, string> = {
  active: 'bg-green-500',
  draft: 'bg-amber-500',
  archived: 'bg-muted-foreground/60',
};

export const PRODUCT_DEFAULT_CURRENCY = 'IQD';
