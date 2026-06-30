'use client';

import { useTransition } from 'react';
import Link from 'next/link';
import { useQueryStates } from 'nuqs';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Plus, Search, X } from 'lucide-react';
import { PageHeader } from '@/components/layout/page-header';
import { ListPageCard } from '@/components/layout/list-page-card';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { BrandTable } from '../blocks/brand-tables';
import { brandColumns } from '../blocks/brand-tables/columns';
import { useBrands } from '../../hooks/useBrand';
import { brandListingParsers, normalizeBrandPageSize } from '../../search-params';
import type { BrandQueryParams } from '../../types';

const m = messages.brand.listing;

export function BrandListingPage() {
  const [, startTransition] = useTransition();

  const [{ page, limit, search }, setParams] = useQueryStates(brandListingParsers, {
    history: 'replace',
    shallow: true,
    startTransition,
  });

  function handleSearchChange(value: string) {
    void setParams(
      { search: value || null, page: null },
      { throttleMs: 300 },
    );
  }

  function handlePageChange(newPage: number) {
    void setParams({ page: newPage });
  }

  function handlePageSizeChange(newLimit: number) {
    void setParams({ limit: newLimit, page: null });
  }

  const pageSize = normalizeBrandPageSize(limit);

  const queryParams: BrandQueryParams = {
    page,
    limit: pageSize,
    ...(search && { search }),
    sortBy: 'name',
    sortOrder: 'asc',
  };

  const { data: listData, isLoading: isListLoading } = useBrands(queryParams);

  const showEmpty =
    !isListLoading &&
    !search &&
    (listData?.items.length ?? 0) === 0;

  return (
    <div className="flex flex-1 flex-col gap-4">
      <PageHeader
        title={m.title}
        description={m.description}
        action={
          <Button id="brands-add-btn" asChild>
            <Link href="/brands/new">
              <Plus className="size-4" />
              {m.addButton}
            </Link>
          </Button>
        }
      />

      <ListPageCard>
        <ListPageCard.Toolbar>
          <div className="relative flex-1 max-w-sm">
            <Search className="text-muted-foreground pointer-events-none absolute top-1/2 start-3 size-4 -translate-y-1/2" />
            <Input
              id="brands-search-input"
              placeholder={m.searchPlaceholder}
              value={search}
              onChange={(e) => handleSearchChange(e.target.value)}
              className={cn('ps-9', search && 'pe-9')}
            />
            {search && (
              <Button
                id="brands-clear-search-btn"
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
        </ListPageCard.Toolbar>

        <ListPageCard.Content>
          <BrandTable
            data={listData?.items ?? []}
            columns={brandColumns}
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
