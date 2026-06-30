import {
  createSearchParamsCache,
  parseAsInteger,
  parseAsString,
  parseAsBoolean,
} from 'nuqs/server';
import { ATTRIBUTE_GROUP_DEFAULT_PAGE_SIZE } from './constants';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50] as const;

export const attributeGroupListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(ATTRIBUTE_GROUP_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(''),
  isFilterable: parseAsBoolean.withDefault(null as unknown as boolean),
};

export function normalizeAttributeGroupPageSize(limit: number): number {
  return PAGE_SIZE_OPTIONS.includes(limit as (typeof PAGE_SIZE_OPTIONS)[number])
    ? limit
    : ATTRIBUTE_GROUP_DEFAULT_PAGE_SIZE;
}

export const attributeGroupListingSearchParamsCache =
  createSearchParamsCache(attributeGroupListingParsers);
