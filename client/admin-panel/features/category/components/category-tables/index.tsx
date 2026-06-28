'use client';

import {
  flexRender,
  getCoreRowModel,
  useReactTable,
  type ColumnDef,
} from '@tanstack/react-table';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { ScrollArea, ScrollBar } from '@/components/ui/scroll-area';
import { Skeleton } from '@/components/ui/skeleton';
import { Button } from '@/components/ui/button';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { messages } from '@/lib/messages.ar';
import { CategoryEmptyState } from '../category-empty-state';
import type { CategoryListItem } from '../../types';

const m = messages.category.listing;

// ─── Props ────────────────────────────────────────────────────────────────────

interface CategoryTableProps {
  data: CategoryListItem[];
  columns: ColumnDef<CategoryListItem>[];
  isLoading?: boolean;
  page: number;
  totalPages: number;
  total: number;
  onPageChange: (page: number) => void;
  embedded?: boolean;
  showEmptyState?: boolean;
}

// ─── Skeleton rows (preserves layout when loading) ───────────────────────────

function SkeletonRows({ columnCount }: { columnCount: number }) {
  return (
    <>
      {Array.from({ length: 8 }).map((_, i) => (
        <TableRow key={i}>
          {Array.from({ length: columnCount }).map((_, j) => (
            <TableCell key={j}>
              <Skeleton className="h-4 w-full" />
            </TableCell>
          ))}
        </TableRow>
      ))}
    </>
  );
}

// ─── Pagination bar ───────────────────────────────────────────────────────────

function PaginationBar({
  page,
  totalPages,
  total,
  isLoading,
  onPageChange,
}: {
  page: number;
  totalPages: number;
  total: number;
  isLoading: boolean;
  onPageChange: (page: number) => void;
}) {
  return (
    <div className="flex items-center justify-between border-t pt-3">
      <div className="text-muted-foreground text-sm">
        {isLoading ? (
          <Skeleton className="inline-block h-4 w-28" />
        ) : (
          <>
            {m.paginationTotal(total)} — {m.paginationPage(page, totalPages)}
          </>
        )}
      </div>
      <div className="flex items-center gap-1">
        <Button
          variant="outline"
          size="icon-sm"
          onClick={() => onPageChange(page - 1)}
          disabled={page <= 1 || isLoading}
          aria-label="الصفحة السابقة"
        >
          <ChevronRight className="size-4" />
        </Button>
        <Button
          variant="outline"
          size="icon-sm"
          onClick={() => onPageChange(page + 1)}
          disabled={page >= totalPages || isLoading}
          aria-label="الصفحة التالية"
        >
          <ChevronLeft className="size-4" />
        </Button>
      </div>
    </div>
  );
}

// ─── Component ────────────────────────────────────────────────────────────────

export function CategoryTable({
  data,
  columns,
  isLoading = false,
  page,
  totalPages,
  total,
  onPageChange,
  embedded = false,
  showEmptyState = false,
}: CategoryTableProps) {
  const table = useReactTable({ data, columns, getCoreRowModel: getCoreRowModel() });

  if (showEmptyState) {
    return <CategoryEmptyState />;
  }

  return (
    <div className="flex flex-1 flex-col gap-3">
      <div className="relative flex flex-1">
        <div
          className={
            embedded
              ? 'absolute inset-0 flex overflow-hidden rounded-lg border border-border/60'
              : 'absolute inset-0 flex overflow-hidden rounded-lg border'
          }
        >
          <ScrollArea className="h-full w-full">
            <Table>
              <TableHeader className="bg-muted sticky top-0 z-10">
                {table.getHeaderGroups().map((hg) => (
                  <TableRow key={hg.id}>
                    {hg.headers.map((h) => (
                      <TableHead key={h.id}>
                        {h.isPlaceholder
                          ? null
                          : flexRender(h.column.columnDef.header, h.getContext())}
                      </TableHead>
                    ))}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {isLoading ? (
                  <SkeletonRows columnCount={columns.length} />
                ) : table.getRowModel().rows.length ? (
                  table.getRowModel().rows.map((row) => (
                    <TableRow key={row.id} className="group">
                      {row.getVisibleCells().map((cell) => (
                        <TableCell key={cell.id}>
                          {flexRender(cell.column.columnDef.cell, cell.getContext())}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell
                      colSpan={columns.length}
                      className="text-muted-foreground h-24 text-center"
                    >
                      لا توجد فئات.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
            <ScrollBar orientation="horizontal" />
          </ScrollArea>
        </div>
      </div>

      {/* ── Pagination — always visible below the scroll area ────────────── */}
      <PaginationBar
        page={page}
        totalPages={totalPages}
        total={total}
        isLoading={isLoading}
        onPageChange={onPageChange}
      />
    </div>
  );
}
