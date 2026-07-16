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
import { ProductTable } from '../blocks/product-tables';
import { productColumns } from '../blocks/product-tables/columns';
import { useProducts } from '../../hooks/useProduct';
import { productListingParsers, normalizeProductPageSize } from '../../search-params';
import type { ProductQueryParams, ProductStatus } from '../../types';

const m = messages.product.listing;
const f = messages.product.form;

export function ProductListingPage() {
  const [, startTransition] = useTransition();

  const [{ page, limit, search, status }, setParams] = useQueryStates(productListingParsers, {
    history: 'replace',
    shallow: true,
    startTransition,
  });

  function handleSearchChange(value: string) {
    void setParams({ search: value || null, page: null }, { throttleMs: 300 });
  }

  function handleStatusChange(value: string) {
    void setParams({
      status: value === 'all' ? null : (value as ProductStatus),
      page: null,
    });
  }

  function handlePageChange(newPage: number) {
    void setParams({ page: newPage });
  }

  function handlePageSizeChange(newLimit: number) {
    void setParams({ limit: newLimit, page: null });
  }

  const pageSize = normalizeProductPageSize(limit);

  const queryParams: ProductQueryParams = {
    page,
    limit: pageSize,
    ...(search && { search }),
    ...(status && { status }),
    sortBy: 'createdAt',
    sortOrder: 'desc',
  };

  const { data: listData, isLoading: isListLoading } = useProducts(queryParams);

  const showEmpty =
    !isListLoading && !search && !status && (listData?.items.length ?? 0) === 0;

  return (
    <div className="flex flex-1 flex-col gap-4">
      <PageHeader
        title={m.title}
        description={m.description}
        action={
          <Button id="products-add-btn" asChild>
            <Link href="/products/new">
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
                id="products-search-input"
                placeholder={m.searchPlaceholder}
                value={search}
                onChange={(e) => handleSearchChange(e.target.value)}
                className={cn('ps-9', search && 'pe-9')}
              />
              {search && (
                <Button
                  id="products-clear-search-btn"
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
              <SelectTrigger className="w-[180px]" id="products-status-filter">
                <SelectValue placeholder={m.filterStatus} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">{m.filterAll}</SelectItem>
                <SelectItem value="active">{f.statusActive}</SelectItem>
                <SelectItem value="draft">{f.statusDraft}</SelectItem>
                <SelectItem value="archived">{f.statusArchived}</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </ListPageCard.Toolbar>

        <ListPageCard.Content>
          <ProductTable
            data={listData?.items ?? []}
            columns={productColumns}
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
