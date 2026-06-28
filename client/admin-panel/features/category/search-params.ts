import {
  createSearchParamsCache,
  parseAsInteger,
  parseAsString,
  parseAsStringEnum,
} from 'nuqs/server';
import { CATEGORY_DEFAULT_PAGE_SIZE } from './constants';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50] as const;

export const categoryListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(CATEGORY_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(''),
  view: parseAsStringEnum(['table', 'tree']).withDefault('table'),
};

export function normalizeCategoryPageSize(limit: number): number {
  return PAGE_SIZE_OPTIONS.includes(limit as (typeof PAGE_SIZE_OPTIONS)[number])
    ? limit
    : CATEGORY_DEFAULT_PAGE_SIZE;
}

export const categoryListingSearchParamsCache =
  createSearchParamsCache(categoryListingParsers);
