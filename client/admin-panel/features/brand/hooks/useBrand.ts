'use client';

import { useQuery } from '@tanstack/react-query';
import { getBrands, getBrandById } from '../api/brand.api';
import type { BrandDto, BrandQueryParams } from '../types';
import { BRAND_QUERY_KEYS } from '../constants';

export const brandKeys = {
  all: BRAND_QUERY_KEYS.ALL,
  lists: () => [...brandKeys.all, 'list'] as const,
  list: (params: BrandQueryParams) => [...brandKeys.lists(), params] as const,
  details: () => [...brandKeys.all, 'detail'] as const,
  detail: (id: number) => [...brandKeys.details(), id] as const,
};

export function useBrands(params: BrandQueryParams = {}, enabled = true) {
  return useQuery({
    queryKey: brandKeys.list(params),
    queryFn: () => getBrands(params),
    placeholderData: (prev) => prev,
    staleTime: 60_000,
    enabled,
  });
}

export function useBrand(
  id: number,
  options?: { enabled?: boolean; initialData?: BrandDto },
) {
  return useQuery({
    queryKey: brandKeys.detail(id),
    queryFn: () => getBrandById(id),
    enabled: (options?.enabled ?? true) && id > 0,
    initialData: options?.initialData ?? undefined,
    staleTime: 60_000,
  });
}
