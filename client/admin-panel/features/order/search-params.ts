import {
  parseAsInteger,
  parseAsString,
  parseAsStringEnum,
} from 'nuqs/server';
import { ORDER_DEFAULT_PAGE_SIZE } from './constants';
import {
  ORDER_FULFILLMENT_STATUSES,
  ORDER_PAYMENT_STATUSES,
  ORDER_STATUSES,
  type OrderFulfillmentStatus,
  type OrderPaymentStatus,
  type OrderStatus,
} from './types';

const PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50] as const;

const statusParser = parseAsStringEnum<OrderStatus>([...ORDER_STATUSES]);
const paymentStatusParser = parseAsStringEnum<OrderPaymentStatus>([...ORDER_PAYMENT_STATUSES]);
const fulfillmentStatusParser = parseAsStringEnum<OrderFulfillmentStatus>([
  ...ORDER_FULFILLMENT_STATUSES,
]);

export const orderListingParsers = {
  page: parseAsInteger.withDefault(1),
  limit: parseAsInteger.withDefault(ORDER_DEFAULT_PAGE_SIZE).withOptions({
    clearOnDefault: true,
  }),
  search: parseAsString.withDefault(''),
  status: statusParser,
  paymentStatus: paymentStatusParser,
  fulfillmentStatus: fulfillmentStatusParser,
};

export function normalizeOrderPageSize(limit: number): number {
  return PAGE_SIZE_OPTIONS.includes(limit as (typeof PAGE_SIZE_OPTIONS)[number])
    ? limit
    : ORDER_DEFAULT_PAGE_SIZE;
}
