// features/category/constants/index.ts
// Feature-specific enums, query keys, and static config

import type { StatusTone } from '@/components/ui/status-indicator';
import type { CategoryStatus } from '../types';

// ─── React Query key namespace ────────────────────────────────────────────────
// Namespaced to prevent key collisions between features

export const CATEGORY_QUERY_KEYS = {
  ALL: ['categories'] as const,
};

// ─── Depth / tree constraints ─────────────────────────────────────────────────

export const CATEGORY_MAX_DEPTH = 5;

// ─── Pagination ───────────────────────────────────────────────────────────────

export const CATEGORY_DEFAULT_PAGE_SIZE = 10;
export const CATEGORY_MAX_PAGE_SIZE = 100;

// ─── Status display config ────────────────────────────────────────────────────

export const CATEGORY_STATUS_TONES: Record<CategoryStatus, StatusTone> = {
  active: 'success',
  inactive: 'neutral',
};

// Tailwind classes keyed by status — used on Badge in table and tree views
// Dark variants match StatusIndicator badge tones for consistent theming
export const CATEGORY_STATUS_COLORS: Record<CategoryStatus, string> = {
  active:
    'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  inactive:
    'bg-gray-50 text-gray-700 border-gray-200 dark:bg-gray-900/40 dark:text-gray-400 dark:border-gray-800',
};

export const CATEGORY_STATUS_DOT_COLORS: Record<CategoryStatus, string> = {
  active: 'bg-green-500',
  inactive: 'bg-muted-foreground/60',
};
