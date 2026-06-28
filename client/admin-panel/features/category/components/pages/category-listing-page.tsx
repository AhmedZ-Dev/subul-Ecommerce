'use client';
// features/category/components/pages/category-listing-page.tsx

import { useEffect, useTransition } from 'react';
import Link from 'next/link';
import { useQueryStates } from 'nuqs';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Plus, Search, List, Network, X } from 'lucide-react';
import { PageHeader } from '@/components/layout/page-header';
import { ListPageCard } from '@/components/layout/list-page-card';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { CategoryTable } from '../blocks/category-tables';
import { categoryColumns } from '../blocks/category-tables/columns';
import { CategoryTree } from '../blocks/category-tree';
import { useCategories } from '../../hooks/useCategory';
import { useCategoryTree } from '../../hooks/useCategory';
import { categoryListingParsers, normalizeCategoryPageSize } from '../../search-params';
import type { CategoryQueryParams } from '../../types';

type ViewMode = 'table' | 'tree';

const m = messages.category.listing;
const VIEW_STORAGE_KEY = 'categories-view-mode';

export function CategoryListingPage() {
  const [, startTransition] = useTransition();

  const [{ page, limit, search, view }, setParams] = useQueryStates(categoryListingParsers, {
    history: 'replace',
    shallow: true,
    startTransition,
  });

  const viewMode: ViewMode = view;

  useEffect(() => {
    const hasViewParam = new URLSearchParams(window.location.search).has('view');
    if (hasViewParam) return;

    const stored = localStorage.getItem(VIEW_STORAGE_KEY);
    if (stored === 'tree') {
      void setParams({ view: 'tree' });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps -- bootstrap localStorage preference once on mount
  }, []);

  function handleSearchChange(value: string) {
    void setParams(
      { search: value || null, page: null },
      { throttleMs: 300 },
    );
  }

  function handleViewChange(mode: ViewMode) {
    localStorage.setItem(VIEW_STORAGE_KEY, mode);
    void setParams({ view: mode === 'tree' ? 'tree' : null });
  }

  function handlePageChange(newPage: number) {
    void setParams({ page: newPage });
  }

  function handlePageSizeChange(newLimit: number) {
    void setParams({ limit: newLimit, page: null });
  }

  const pageSize = normalizeCategoryPageSize(limit);

  const queryParams: CategoryQueryParams = {
    page,
    limit: pageSize,
    ...(search && { search }),
    sortBy: 'nameEn',
    sortOrder: 'asc',
  };

  const { data: listData, isLoading: isListLoading } = useCategories(
    queryParams,
    viewMode === 'table',
  );
  const { data: treeData, isLoading: isTreeLoading } = useCategoryTree(
    viewMode === 'tree',
  );

  const showEmpty =
    viewMode === 'table' &&
    !isListLoading &&
    !search &&
    (listData?.items.length ?? 0) === 0;

  return (
    <div className="flex flex-1 flex-col gap-4">
      <PageHeader
        title={m.title}
        description={m.description}
        action={
          <Button id="categories-add-btn" asChild>
            <Link href="/categories/new">
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
              id="categories-search-input"
              placeholder={m.searchPlaceholder}
              value={search}
              onChange={(e) => handleSearchChange(e.target.value)}
              className={cn('ps-9', search && 'pe-9')}
            />
            {search && (
              <Button
                id="categories-clear-search-btn"
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

          <div
            className="flex items-center rounded-lg border p-1"
            role="group"
            aria-label={m.viewModeLabel}
          >
            <Button
              id="categories-table-view-btn"
              type="button"
              variant={viewMode === 'table' ? 'default' : 'ghost'}
              size="sm"
              onClick={() => handleViewChange('table')}
              aria-pressed={viewMode === 'table'}
            >
              <List className="size-4" />
              {m.viewTable}
            </Button>
            <Button
              id="categories-tree-view-btn"
              type="button"
              variant={viewMode === 'tree' ? 'default' : 'ghost'}
              size="sm"
              onClick={() => handleViewChange('tree')}
              aria-pressed={viewMode === 'tree'}
            >
              <Network className="size-4" />
              {m.viewTree}
            </Button>
          </div>
        </ListPageCard.Toolbar>

        <ListPageCard.Content>
          {viewMode === 'table' ? (
            <CategoryTable
              data={listData?.items ?? []}
              columns={categoryColumns}
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
          ) : (
            <CategoryTree nodes={treeData ?? []} isLoading={isTreeLoading} />
          )}
        </ListPageCard.Content>
      </ListPageCard>
    </div>
  );
}
