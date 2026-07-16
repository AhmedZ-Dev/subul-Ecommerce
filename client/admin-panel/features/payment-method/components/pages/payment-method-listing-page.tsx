'use client';

import { useTransition } from 'react';
import Link from 'next/link';
import { useQueryStates } from 'nuqs';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Plus, Search, X } from 'lucide-react';
import { PageHeader } from '@/components/layout/page-header';
import { ListPageCard } from '@/components/layout/list-page-card';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { PaymentMethodTable } from '../blocks/payment-method-tables';
import { paymentMethodColumns } from '../blocks/payment-method-tables/columns';
import { usePaymentMethods } from '../../hooks/usePaymentMethod';
import {
  paymentMethodListingParsers,
  normalizePaymentMethodPageSize,
} from '../../search-params';
import type { PaymentMethodQueryParams, PaymentMethodType } from '../../types';

const m = messages.paymentMethod.listing;
const typeLabels = messages.paymentMethod.type;
const statusLabels = messages.paymentMethod.status;

export function PaymentMethodListingPage() {
  const [, startTransition] = useTransition();

  const [{ page, limit, search, type, status }, setParams] = useQueryStates(
    paymentMethodListingParsers,
    {
      history: 'replace',
      shallow: true,
      startTransition,
    },
  );

  function handleSearchChange(value: string) {
    void setParams({ search: value || null, page: null }, { throttleMs: 300 });
  }

  function handlePageChange(newPage: number) {
    void setParams({ page: newPage });
  }

  function handlePageSizeChange(newLimit: number) {
    void setParams({ limit: newLimit, page: null });
  }

  const pageSize = normalizePaymentMethodPageSize(limit);

  const queryParams: PaymentMethodQueryParams = {
    page,
    limit: pageSize,
    ...(search && { search }),
    ...(type && { type: type as PaymentMethodType }),
    ...(status && { status }),
    sortBy: 'sortOrder',
    sortOrder: 'asc',
  };

  const { data: listData, isLoading: isListLoading } = usePaymentMethods(queryParams);

  const showEmpty =
    !isListLoading &&
    !search &&
    !type &&
    !status &&
    (listData?.items.length ?? 0) === 0;

  return (
    <div className="flex flex-1 flex-col gap-4">
      <PageHeader
        title={m.title}
        description={m.description}
        action={
          <Button id="payment-methods-add-btn" asChild>
            <Link href="/payment-methods/new">
              <Plus className="size-4" />
              {m.addButton}
            </Link>
          </Button>
        }
      />

      <ListPageCard>
        <ListPageCard.Toolbar>
          <div className="flex flex-1 flex-wrap items-center gap-3">
            <div className="relative max-w-sm flex-1">
              <Search className="text-muted-foreground pointer-events-none absolute top-1/2 start-3 size-4 -translate-y-1/2" />
              <Input
                id="payment-methods-search-input"
                placeholder={m.searchPlaceholder}
                value={search}
                onChange={(e) => handleSearchChange(e.target.value)}
                className={cn('ps-9', search && 'pe-9')}
              />
              {search && (
                <Button
                  id="payment-methods-clear-search-btn"
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

            <Select
              value={type ?? 'all'}
              onValueChange={(value) =>
                void setParams({
                  type: value === 'all' ? null : (value as PaymentMethodType),
                  page: null,
                })
              }
            >
              <SelectTrigger id="payment-methods-type-filter" className="w-[160px]">
                <SelectValue placeholder={m.filterType} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{m.filterTypeAll}</SelectItem>
                <SelectItem value="offline">{typeLabels.offline}</SelectItem>
                <SelectItem value="online">{typeLabels.online}</SelectItem>
              </SelectContent>
            </Select>

            <Select
              value={status ?? 'all'}
              onValueChange={(value) =>
                void setParams({
                  status: value === 'all' ? null : (value as 'active' | 'inactive'),
                  page: null,
                })
              }
            >
              <SelectTrigger id="payment-methods-status-filter" className="w-[160px]">
                <SelectValue placeholder={m.filterStatus} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{m.filterStatusAll}</SelectItem>
                <SelectItem value="active">{statusLabels.active}</SelectItem>
                <SelectItem value="inactive">{statusLabels.inactive}</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </ListPageCard.Toolbar>

        <ListPageCard.Content>
          <PaymentMethodTable
            data={listData?.items ?? []}
            columns={paymentMethodColumns}
            isLoading={isListLoading}
            page={page}
            pageSize={pageSize}
            totalPages={listData?.totalPages ?? 1}
            total={listData?.total ?? 0}
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
            embedded
            showEmptyState={showEmpty}
          />
        </ListPageCard.Content>
      </ListPageCard>
    </div>
  );
}
