'use client';

import { useTransition } from 'react';
import { useQueryStates } from 'nuqs';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Search, X } from 'lucide-react';
import { PageHeader } from '@/components/layout/page-header';
import { ListPageCard } from '@/components/layout/list-page-card';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { OrderTable } from '../blocks/order-tables';
import { orderColumns } from '../blocks/order-tables/columns';
import { useOrders } from '../../hooks/useOrder';
import { orderListingParsers, normalizeOrderPageSize } from '../../search-params';
import {
  ORDER_FULFILLMENT_STATUSES,
  ORDER_PAYMENT_STATUSES,
  ORDER_STATUSES,
  type OrderFulfillmentStatus,
  type OrderPaymentStatus,
  type OrderQueryParams,
  type OrderStatus,
} from '../../types';

const m = messages.order.listing;
const statusLabels = messages.order.status;
const paymentLabels = messages.order.paymentStatus;
const fulfillmentLabels = messages.order.fulfillmentStatus;

export function OrderListingPage() {
  const [, startTransition] = useTransition();

  const [
    { page, limit, search, status, paymentStatus, fulfillmentStatus },
    setParams,
  ] = useQueryStates(orderListingParsers, {
    history: 'replace',
    shallow: true,
    startTransition,
  });

  function handleSearchChange(value: string) {
    void setParams({ search: value || null, page: null }, { throttleMs: 300 });
  }

  function handleStatusChange(value: string) {
    void setParams({
      status: value === 'all' ? null : (value as OrderStatus),
      page: null,
    });
  }

  function handlePaymentStatusChange(value: string) {
    void setParams({
      paymentStatus: value === 'all' ? null : (value as OrderPaymentStatus),
      page: null,
    });
  }

  function handleFulfillmentStatusChange(value: string) {
    void setParams({
      fulfillmentStatus: value === 'all' ? null : (value as OrderFulfillmentStatus),
      page: null,
    });
  }

  function handlePageChange(newPage: number) {
    void setParams({ page: newPage });
  }

  function handlePageSizeChange(newLimit: number) {
    void setParams({ limit: newLimit, page: null });
  }

  const pageSize = normalizeOrderPageSize(limit);

  const queryParams: OrderQueryParams = {
    page,
    limit: pageSize,
    ...(search && { search }),
    ...(status && { status }),
    ...(paymentStatus && { paymentStatus }),
    ...(fulfillmentStatus && { fulfillmentStatus }),
    sortBy: 'createdAt',
    sortOrder: 'desc',
  };

  const { data: listData, isLoading: isListLoading } = useOrders(queryParams);

  return (
    <div className="flex flex-1 flex-col gap-4">
      <PageHeader title={m.title} description={m.description} />

      <ListPageCard>
        <ListPageCard.Toolbar>
          <div className="flex flex-1 flex-wrap items-center gap-2">
            <div className="relative min-w-[200px] max-w-sm flex-1">
              <Search className="text-muted-foreground pointer-events-none absolute top-1/2 start-3 size-4 -translate-y-1/2" />
              <Input
                id="orders-search-input"
                placeholder={m.searchPlaceholder}
                value={search}
                onChange={(e) => handleSearchChange(e.target.value)}
                className={cn('ps-9', search && 'pe-9')}
              />
              {search && (
                <Button
                  id="orders-clear-search-btn"
                  type="button"
                  variant="ghost"
                  size="icon-xs"
                  className="absolute top-1/2 end-1 -translate-y-1/2"
                  onClick={() => void setParams({ search: null, page: null })}
                  aria-label={m.clearSearch}
                >
                  <X className="size-3.5" />
                </Button>
              )}
            </div>

            <Select value={status ?? 'all'} onValueChange={handleStatusChange}>
              <SelectTrigger className="w-[160px]" id="orders-status-filter">
                <SelectValue placeholder={m.filterStatus} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{m.filterAll}</SelectItem>
                {ORDER_STATUSES.map((s) => (
                  <SelectItem key={s} value={s}>
                    {statusLabels[s]}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            <Select value={paymentStatus ?? 'all'} onValueChange={handlePaymentStatusChange}>
              <SelectTrigger className="w-[160px]" id="orders-payment-status-filter">
                <SelectValue placeholder={m.filterPaymentStatus} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{m.filterAll}</SelectItem>
                {ORDER_PAYMENT_STATUSES.map((s) => (
                  <SelectItem key={s} value={s}>
                    {paymentLabels[s]}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            <Select
              value={fulfillmentStatus ?? 'all'}
              onValueChange={handleFulfillmentStatusChange}
            >
              <SelectTrigger className="w-[160px]" id="orders-fulfillment-status-filter">
                <SelectValue placeholder={m.filterFulfillmentStatus} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{m.filterAll}</SelectItem>
                {ORDER_FULFILLMENT_STATUSES.map((s) => (
                  <SelectItem key={s} value={s}>
                    {fulfillmentLabels[s]}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </ListPageCard.Toolbar>

        <ListPageCard.Content>
          <OrderTable
            data={listData?.items ?? []}
            columns={orderColumns}
            isLoading={isListLoading}
            page={page}
            pageSize={pageSize}
            totalPages={listData?.totalPages ?? 1}
            total={listData?.total ?? 0}
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
            embedded
          />
        </ListPageCard.Content>
      </ListPageCard>
    </div>
  );
}
