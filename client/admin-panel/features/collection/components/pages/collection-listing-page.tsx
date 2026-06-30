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
import { CollectionTable } from '../blocks/collection-tables';
import { collectionColumns } from '../blocks/collection-tables/columns';
import { useCollections } from '../../hooks/useCollection';
import { collectionListingParsers, normalizeCollectionPageSize } from '../../search-params';
import type { CollectionQueryParams } from '../../types';

const m = messages.collection.listing;
const f = messages.collection.form;

export function CollectionListingPage() {
  const [, startTransition] = useTransition();

  const [{ page, limit, search, type }, setParams] = useQueryStates(collectionListingParsers, {
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

  function handleTypeChange(value: string) {
    void setParams({ type: value === 'all' ? null : (value as 'manual' | 'smart'), page: null });
  }

  function handlePageChange(newPage: number) {
    void setParams({ page: newPage });
  }

  function handlePageSizeChange(newLimit: number) {
    void setParams({ limit: newLimit, page: null });
  }

  const pageSize = normalizeCollectionPageSize(limit);

  const queryParams: CollectionQueryParams = {
    page,
    limit: pageSize,
    ...(search && { search }),
    ...(type && { type }),
    sortBy: 'sortOrder',
    sortOrder: 'asc',
  };

  const { data: listData, isLoading: isListLoading } = useCollections(queryParams);

  const showEmpty =
    !isListLoading &&
    !search &&
    !type &&
    (listData?.items.length ?? 0) === 0;

  return (
    <div className="flex flex-1 flex-col gap-4">
      <PageHeader
        title={m.title}
        description={m.description}
        action={
          <Button id="collections-add-btn" asChild>
            <Link href="/collections/new">
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
                id="collections-search-input"
                placeholder={m.searchPlaceholder}
                value={search}
                onChange={(e) => handleSearchChange(e.target.value)}
                className={cn('ps-9', search && 'pe-9')}
              />
              {search && (
                <Button
                  id="collections-clear-search-btn"
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

            <Select value={type ?? 'all'} onValueChange={handleTypeChange}>
              <SelectTrigger className="w-[180px]" id="collections-type-filter">
                <SelectValue placeholder={f.type} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">الكل</SelectItem>
                <SelectItem value="manual">{f.typeManual}</SelectItem>
                <SelectItem value="smart">{f.typeSmart}</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </ListPageCard.Toolbar>

        <ListPageCard.Content>
          <CollectionTable
            data={listData?.items ?? []}
            columns={collectionColumns}
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
