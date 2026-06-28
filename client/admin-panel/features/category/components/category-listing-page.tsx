'use client';
// features/category/components/category-listing-page.tsx

import { useState, useTransition, useCallback, useEffect } from 'react';
import Link from 'next/link';
import { useRouter, usePathname, useSearchParams } from 'next/navigation';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Plus, Search, List, Network, X } from 'lucide-react';
import { PageHeader } from '@/components/layout/page-header';
import { ListPageCard } from '@/components/layout/list-page-card';
import { useDebounce } from '@/hooks/use-debounce';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { CategoryTable } from './category-tables';
import { categoryColumns } from './category-tables/columns';
import { CategoryTree } from './category-tree';
import { useCategories } from '../hooks/useCategory';
import { useCategoryTree } from '../hooks/useCategory';
import { CATEGORY_DEFAULT_PAGE_SIZE } from '../constants';
import type { CategoryQueryParams } from '../types';

type ViewMode = 'table' | 'tree';

const m = messages.category.listing;
const VIEW_STORAGE_KEY = 'categories-view-mode';

function parseViewMode(value: string | null): ViewMode {
  return value === 'tree' ? 'tree' : 'table';
}

export function CategoryListingPage() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const [_isPending, startTransition] = useTransition();

  const page = parseInt(searchParams.get('page') ?? '1', 10);
  const search = searchParams.get('search') ?? '';
  const viewFromUrl = searchParams.get('view');
  const [localSearch, setLocalSearch] = useState(search);
  const [viewMode, setViewMode] = useState<ViewMode>('table');

  const debouncedSearch = useDebounce(localSearch, 300);

  useEffect(() => {
    if (viewFromUrl) {
      setViewMode(parseViewMode(viewFromUrl));
      return;
    }
    const stored = localStorage.getItem(VIEW_STORAGE_KEY);
    if (stored === 'tree' || stored === 'table') {
      setViewMode(stored);
    }
  }, [viewFromUrl]);

  const updateParams = useCallback(
    (updates: Record<string, string | null>) => {
      const params = new URLSearchParams(searchParams.toString());
      for (const [key, value] of Object.entries(updates)) {
        if (value === null || value === '') {
          params.delete(key);
        } else {
          params.set(key, value);
        }
      }
      startTransition(() => {
        router.replace(`${pathname}?${params.toString()}`);
      });
    },
    [searchParams, pathname, router],
  );

  useEffect(() => {
    setLocalSearch(search);
  }, [search]);

  useEffect(() => {
    if (debouncedSearch === search) return;
    updateParams({ search: debouncedSearch || null, page: null });
  }, [debouncedSearch, search, updateParams]);

  function handleViewChange(mode: ViewMode) {
    setViewMode(mode);
    localStorage.setItem(VIEW_STORAGE_KEY, mode);
    updateParams({ view: mode === 'table' ? null : mode });
  }

  function handlePageChange(newPage: number) {
    updateParams({ page: String(newPage) });
  }

  const queryParams: CategoryQueryParams = {
    page,
    limit: CATEGORY_DEFAULT_PAGE_SIZE,
    ...(search && { search }),
    sortBy: 'nameEn',
    sortOrder: 'asc',
  };

  const { data: listData, isLoading: isListLoading } = useCategories(queryParams);
  const { data: treeData, isLoading: isTreeLoading } = useCategoryTree();

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
              value={localSearch}
              onChange={(e) => setLocalSearch(e.target.value)}
              className={cn('ps-9', localSearch && 'pe-9')}
            />
            {localSearch && (
              <Button
                id="categories-clear-search-btn"
                type="button"
                variant="ghost"
                size="icon-xs"
                className="absolute top-1/2 end-1 -translate-y-1/2"
                onClick={() => setLocalSearch('')}
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
              totalPages={listData?.totalPages ?? 1}
              total={listData?.total ?? 0}
              onPageChange={handlePageChange}
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
