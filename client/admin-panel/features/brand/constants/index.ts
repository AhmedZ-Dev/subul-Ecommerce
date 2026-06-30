// features/brand/constants/index.ts

import type { StatusTone } from '@/components/ui/status-indicator';
import type { BrandStatus } from '../types';

export const BRAND_QUERY_KEYS = {
  ALL: ['brands'] as const,
};

export const BRAND_DEFAULT_PAGE_SIZE = 10;
export const BRAND_MAX_PAGE_SIZE = 100;

export const BRAND_STATUS_TONES: Record<BrandStatus, StatusTone> = {
  active: 'success',
  inactive: 'neutral',
};

export const BRAND_STATUS_COLORS: Record<BrandStatus, string> = {
  active:
    'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  inactive:
    'bg-gray-50 text-gray-700 border-gray-200 dark:bg-gray-900/40 dark:text-gray-400 dark:border-gray-800',
};

export const BRAND_STATUS_DOT_COLORS: Record<BrandStatus, string> = {
  active: 'bg-green-500',
  inactive: 'bg-muted-foreground/60',
};
