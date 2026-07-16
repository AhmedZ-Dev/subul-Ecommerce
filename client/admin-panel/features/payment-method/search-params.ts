import {
  createSearchParamsCache,
  parseAsInteger,
  parseAsString,
  parseAsStringEnum,
} from 'nuqs/server';
import { PAYMENT_METHOD_DEFAULT_PAGE_SIZE } from './constants';
import type { PaymentMethodType } from './types';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50] as const;

export const paymentMethodListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(PAYMENT_METHOD_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(''),
  type: parseAsStringEnum(['offline', 'online'] as PaymentMethodType[]),
  status: parseAsStringEnum(['active', 'inactive'] as const),
};

export function normalizePaymentMethodPageSize(limit: number): number {
  return PAGE_SIZE_OPTIONS.includes(limit as (typeof PAGE_SIZE_OPTIONS)[number])
    ? limit
    : PAYMENT_METHOD_DEFAULT_PAGE_SIZE;
}

export const paymentMethodListingSearchParamsCache =
  createSearchParamsCache(paymentMethodListingParsers);
