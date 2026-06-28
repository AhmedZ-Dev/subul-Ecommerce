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

export const CATEGORY_STATUS_OPTIONS: Array<{ value: CategoryStatus; label: string }> = [
  { value: 'active', label: 'نشط' },
  { value: 'inactive', label: 'غير نشط' },
];

export const CATEGORY_STATUS_TONES: Record<CategoryStatus, StatusTone> = {
  active: 'success',
  inactive: 'neutral',
};

// Tailwind classes keyed by status — used on Badge in table and tree views
export const CATEGORY_STATUS_COLORS: Record<CategoryStatus, string> = {
  active: 'bg-green-50 text-green-700 border-green-200',
  inactive: 'bg-gray-50 text-gray-700 border-gray-200',
};

export const CATEGORY_STATUS_DOT_COLORS: Record<CategoryStatus, string> = {
  active: 'bg-green-500',
  inactive: 'bg-muted-foreground/60',
};
