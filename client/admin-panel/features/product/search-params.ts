import {
  createSearchParamsCache,
  parseAsBoolean,
  parseAsInteger,
  parseAsString,
  parseAsStringEnum,
} from 'nuqs/server';
import { PRODUCT_DEFAULT_PAGE_SIZE } from './constants';
import type { ProductStatus } from './types';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50] as const;

const statusParser = parseAsStringEnum<ProductStatus>(['active', 'draft', 'archived']);

export const productListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(PRODUCT_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(''),
  status: statusParser,
  categoryId: parseAsInteger,
  brandId: parseAsInteger,
  isFeatured: parseAsBoolean,
};

export function normalizeProductPageSize(limit: number): number {
  return PAGE_SIZE_OPTIONS.includes(limit as (typeof PAGE_SIZE_OPTIONS)[number])
    ? limit
    : PRODUCT_DEFAULT_PAGE_SIZE;
}

export const productListingSearchParamsCache =
  createSearchParamsCache(productListingParsers);
