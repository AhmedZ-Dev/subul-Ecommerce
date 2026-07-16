import {
  createSearchParamsCache,
  createSerializer,
  parseAsInteger,
  parseAsString,
  parseAsStringEnum,
} from 'nuqs/server';

import { SHIPPING_ZONE_DEFAULT_PAGE_SIZE } from './constants';
import type { ShippingZoneStatus } from './types';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50] as const;

const statusParser = parseAsStringEnum<ShippingZoneStatus>(['active', 'inactive']);
const sortOrderParser = parseAsStringEnum<'asc' | 'desc'>(['asc', 'desc']);
const sortByParser = parseAsStringEnum<'nameEn' | 'createdAt'>(['nameEn', 'createdAt']);

export const shippingZoneListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(SHIPPING_ZONE_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(''),
  status: statusParser,
  sortBy: sortByParser.withDefault('createdAt'),
  sortOrder: sortOrderParser.withDefault('desc'),
};

export function normalizeShippingZonePageSize(limit: number): number {
  return PAGE_SIZE_OPTIONS.includes(limit as (typeof PAGE_SIZE_OPTIONS)[number])
    ? limit
    : SHIPPING_ZONE_DEFAULT_PAGE_SIZE;
}

export const shippingZoneListingSearchParamsCache =
  createSearchParamsCache(shippingZoneListingParsers);

export const serializeShippingZoneSearchParams =
  createSerializer(shippingZoneListingParsers);
