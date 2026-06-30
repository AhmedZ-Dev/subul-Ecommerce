// features/collection/constants/index.ts

import type { StatusTone } from '@/components/ui/status-indicator';
import type { CollectionStatus, CollectionType } from '../types';

export const COLLECTION_QUERY_KEYS = {
  ALL: ['collections'] as const,
};

export const COLLECTION_DEFAULT_PAGE_SIZE = 10;
export const COLLECTION_MAX_PAGE_SIZE = 100;

export const COLLECTION_STATUS_TONES: Record<CollectionStatus, StatusTone> = {
  active: 'success',
  inactive: 'neutral',
};

export const COLLECTION_STATUS_COLORS: Record<CollectionStatus, string> = {
  active:
    'bg-green-50 text-green-700 border-green-200 dark:bg-green-950/40 dark:text-green-400 dark:border-green-900',
  inactive:
    'bg-gray-50 text-gray-700 border-gray-200 dark:bg-gray-900/40 dark:text-gray-400 dark:border-gray-800',
};

export const COLLECTION_STATUS_DOT_COLORS: Record<CollectionStatus, string> = {
  active: 'bg-green-500',
  inactive: 'bg-muted-foreground/60',
};

export const COLLECTION_TYPE_COLORS: Record<CollectionType, string> = {
  manual:
    'bg-blue-50 text-blue-700 border-blue-200 dark:bg-blue-950/40 dark:text-blue-400 dark:border-blue-900',
  smart:
    'bg-purple-50 text-purple-700 border-purple-200 dark:bg-purple-950/40 dark:text-purple-400 dark:border-purple-900',
};
