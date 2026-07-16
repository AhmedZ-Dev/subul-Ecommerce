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
import { ShippingZoneTable } from '../blocks/shipping-zone-tables';
import { shippingZoneColumns } from '../blocks/shipping-zone-tables/columns';
import { useShippingZones } from '../../hooks/useShippingZone';
import { shippingZoneListingParsers, normalizeShippingZonePageSize } from '../../search-params';
import type { ShippingZoneQueryParams } from '../../types';

const m = messages.shippingZone.listing;
const f = messages.shippingZone.form;

export function ShippingZoneListingPage() {
  const [, startTransition] = useTransition();

  const [{ page, limit, search, status, sortBy, sortOrder }, setParams] = useQueryStates(
    shippingZoneListingParsers,
    {
      history: 'replace',
      shallow: true,
      startTransition,
    },
  );

  function handleSearchChange(value: string) {
    void setParams({ search: value || null, page: null }, { throttleMs: 300 });
  }

  function handleStatusChange(value: string) {
    void setParams({
      status: value === 'all' ? null : (value as 'active' | 'inactive'),
      page: null,
    });
  }

  function handlePageChange(newPage: number) {
    void setParams({ page: newPage });
  }

  function handlePageSizeChange(newLimit: number) {
    void setParams({ limit: newLimit, page: null });
  }

  const pageSize = normalizeShippingZonePageSize(limit);

  const queryParams: ShippingZoneQueryParams = {
    page,
    limit: pageSize,
    ...(search && { search }),
    ...(status && { status }),
    sortBy,
    sortOrder,
  };

  const { data: listData, isLoading: isListLoading } = useShippingZones(queryParams);

  const showEmpty =
    !isListLoading && !search && !status && (listData?.items.length ?? 0) === 0;

  return (
    <div className="flex flex-1 flex-col gap-4">
      <PageHeader
        title={m.title}
        description={m.description}
        action={
          <Button id="shipping-zones-add-btn" asChild>
            <Link href="/shipping-zones/new">
              <Plus className="size-4" />
              {m.addButton}
            </Link>
          </Button>
        }
      />

      <ListPageCard>
        <ListPageCard.Toolbar>
          <div className="flex flex-1 items-center gap-2">
            <div className="relative max-w-sm flex-1">
              <Search className="text-muted-foreground pointer-events-none absolute top-1/2 start-3 size-4 -translate-y-1/2" />
              <Input
                id="shipping-zones-search-input"
                placeholder={m.searchPlaceholder}
                value={search}
                onChange={(e) => handleSearchChange(e.target.value)}
                className={cn('ps-9', search && 'pe-9')}
              />
              {search && (
                <Button
                  id="shipping-zones-clear-search-btn"
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
              <SelectTrigger className="w-[180px]" id="shipping-zones-status-filter">
                <SelectValue placeholder={f.status} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{m.statusAll}</SelectItem>
                <SelectItem value="active">{f.statusActive}</SelectItem>
                <SelectItem value="inactive">{f.statusInactive}</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </ListPageCard.Toolbar>

        <ListPageCard.Content>
          <ShippingZoneTable
            data={listData?.items ?? []}
            columns={shippingZoneColumns}
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
