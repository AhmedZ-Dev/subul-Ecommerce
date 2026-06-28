'use client';
// features/category/hooks/useCategory.ts
// React Query query hooks (read-only). Components import from here — never call API directly.

import { useQuery } from '@tanstack/react-query';
import { getCategories, getCategoryById } from '../api/category.api';
import type { CategoryQueryParams } from '../types';
import { buildCategoryTree } from '../utils';
import { CATEGORY_QUERY_KEYS } from '../constants';

// ─── Query Key Factory ────────────────────────────────────────────────────────
// Centralising keys prevents typos and enables precise cache invalidation

export const categoryKeys = {
  all: CATEGORY_QUERY_KEYS.ALL,
  lists: () => [...categoryKeys.all, 'list'] as const,
  list: (params: CategoryQueryParams) => [...categoryKeys.lists(), params] as const,
  details: () => [...categoryKeys.all, 'detail'] as const,
  detail: (id: number) => [...categoryKeys.details(), id] as const,
};

// ─── useCategories ────────────────────────────────────────────────────────────

export function useCategories(params: CategoryQueryParams = {}) {
  return useQuery({
    queryKey: categoryKeys.list(params),
    queryFn: () => getCategories(params),
    placeholderData: (prev) => prev, // keeps stale data visible while fetching next page
    staleTime: 60_000, // categories change rarely
  });
}

// ─── useCategory ──────────────────────────────────────────────────────────────

export function useCategory(id: number, enabled = true) {
  return useQuery({
    queryKey: categoryKeys.detail(id),
    queryFn: () => getCategoryById(id),
    enabled: enabled && id > 0,
    staleTime: 60_000,
  });
}

// ─── useCategoryTree ──────────────────────────────────────────────────────────
// Derived query — fetches all categories and transforms flat list into tree on client

export function useCategoryTree() {
  const query = useCategories({ limit: 1000, sortBy: 'nameEn', sortOrder: 'asc' });
  return {
    ...query,
    data: query.data ? buildCategoryTree(query.data.items) : undefined,
  };
}
