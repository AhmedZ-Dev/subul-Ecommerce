import {
  createSearchParamsCache,
  createSerializer,
  parseAsInteger,
  parseAsString,
  parseAsStringEnum,
} from 'nuqs/server';

import { COLLECTION_DEFAULT_PAGE_SIZE } from './constants';
import { CollectionStatus, CollectionType } from './types';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50] as const;

const statusParser = parseAsStringEnum<CollectionStatus>(['active', 'inactive']);
const typeParser = parseAsStringEnum<CollectionType>(['manual', 'smart']);
const sortOrderParser = parseAsStringEnum<'asc' | 'desc'>(['asc', 'desc']);
const sortByParser = parseAsStringEnum<'nameEn' | 'createdAt' | 'sortOrder'>([
  'nameEn',
  'createdAt',
  'sortOrder',
]);

export const collectionListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(COLLECTION_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(''),
  status: statusParser,
  type: typeParser,
  sortBy: sortByParser.withDefault('sortOrder'),
  sortOrder: sortOrderParser.withDefault('asc'),
};

export function normalizeCollectionPageSize(limit: number): number {
  return PAGE_SIZE_OPTIONS.includes(limit as (typeof PAGE_SIZE_OPTIONS)[number])
    ? limit
    : COLLECTION_DEFAULT_PAGE_SIZE;
}

export const collectionListingSearchParamsCache =
  createSearchParamsCache(collectionListingParsers);

export const serializeCollectionSearchParams =
  createSerializer(collectionListingParsers);
