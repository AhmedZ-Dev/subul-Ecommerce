import {
  createSearchParamsCache,
  parseAsInteger,
  parseAsString,
} from 'nuqs/server';
import { BRAND_DEFAULT_PAGE_SIZE } from './constants';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50] as const;

export const brandListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(BRAND_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(''),
};

export function normalizeBrandPageSize(limit: number): number {
  return PAGE_SIZE_OPTIONS.includes(limit as (typeof PAGE_SIZE_OPTIONS)[number])
    ? limit
    : BRAND_DEFAULT_PAGE_SIZE;
}

export const brandListingSearchParamsCache =
  createSearchParamsCache(brandListingParsers);
