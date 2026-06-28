'use client';

import {
  ChevronLeftIcon,
  ChevronRightIcon,
  ChevronsLeftIcon,
  ChevronsRightIcon,
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Skeleton } from '@/components/ui/skeleton';
import { messages } from '@/lib/messages.ar';

const DEFAULT_PAGE_SIZE_OPTIONS = [10, 20, 30, 40, 50];

export interface DataTablePaginationProps {
  page: number;
  totalPages: number;
  total: number;
  pageSize: number;
  pageSizeOptions?: number[];
  isLoading?: boolean;
  onPageChange: (page: number) => void;
  onPageSizeChange: (size: number) => void;
  totalLabel?: (total: number) => string;
}

export function DataTablePagination({
  page,
  totalPages,
  total,
  pageSize,
  pageSizeOptions = DEFAULT_PAGE_SIZE_OPTIONS,
  isLoading = false,
  onPageChange,
  onPageSizeChange,
  totalLabel = messages.category.listing.paginationTotal,
}: DataTablePaginationProps) {
  const canGoPrevious = page > 1;
  const canGoNext = page < totalPages;
  const safeTotalPages = totalPages || 1;

  return (
    <div className="flex items-center justify-between border-t pt-3">
      <div className="hidden flex-1 text-sm text-muted-foreground lg:flex">
        {isLoading ? (
          <Skeleton className="inline-block h-4 w-28" />
        ) : (
          totalLabel(total)
        )}
      </div>
      <div className="flex w-full items-center gap-8 lg:w-fit">
        <div className="hidden items-center gap-2 lg:flex">
          <Label htmlFor="rows-per-page" className="text-sm font-medium">
            {messages.table.rowsPerPage}
          </Label>
          <Select
            value={`${pageSize}`}
            onValueChange={(value) => onPageSizeChange(Number(value))}
            disabled={isLoading}
          >
            <SelectTrigger size="sm" className="w-20" id="rows-per-page">
              <SelectValue placeholder={pageSize} />
            </SelectTrigger>
            <SelectContent side="top">
              <SelectGroup>
                {pageSizeOptions.map((size) => (
                  <SelectItem key={size} value={`${size}`}>
                    {size}
                  </SelectItem>
                ))}
              </SelectGroup>
            </SelectContent>
          </Select>
        </div>
        <div className="flex w-fit items-center justify-center text-sm font-medium">
          {isLoading ? (
            <Skeleton className="inline-block h-4 w-24" />
          ) : (
            messages.table.pageOf(page, safeTotalPages)
          )}
        </div>
        <div className="ms-auto flex items-center gap-2 lg:ms-0">
          <Button
            variant="outline"
            className="hidden size-8 lg:flex"
            size="icon"
            onClick={() => onPageChange(1)}
            disabled={!canGoPrevious || isLoading}
          >
            <span className="sr-only">{messages.table.goToFirstPage}</span>
            <ChevronsLeftIcon className="rtl:rotate-180" />
          </Button>
          <Button
            variant="outline"
            className="size-8"
            size="icon"
            onClick={() => onPageChange(page - 1)}
            disabled={!canGoPrevious || isLoading}
          >
            <span className="sr-only">{messages.table.goToPreviousPage}</span>
            <ChevronLeftIcon className="rtl:rotate-180" />
          </Button>
          <Button
            variant="outline"
            className="size-8"
            size="icon"
            onClick={() => onPageChange(page + 1)}
            disabled={!canGoNext || isLoading}
          >
            <span className="sr-only">{messages.table.goToNextPage}</span>
            <ChevronRightIcon className="rtl:rotate-180" />
          </Button>
          <Button
            variant="outline"
            className="hidden size-8 lg:flex"
            size="icon"
            onClick={() => onPageChange(safeTotalPages)}
            disabled={!canGoNext || isLoading}
          >
            <span className="sr-only">{messages.table.goToLastPage}</span>
            <ChevronsRightIcon className="rtl:rotate-180" />
          </Button>
        </div>
      </div>
    </div>
  );
}
