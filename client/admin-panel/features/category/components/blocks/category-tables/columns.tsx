'use client';

import type { ColumnDef } from '@tanstack/react-table';
import { Badge } from '@/components/ui/badge';
import { EntityCell } from '@/components/ui/entity-cell';
import { FolderOpen, FolderTree } from 'lucide-react';
import type { CategoryListItem } from '../../../types';
import { CategoryCellAction } from './cell-action';
import { CategoryStatusToggle } from '../category-status-toggle';

export const categoryColumns: ColumnDef<CategoryListItem>[] = [
  {
    accessorKey: 'nameEn',
    header: 'الاسم',
    cell: ({ row }) => (
      <EntityCell
        title={row.original.nameEn}
        subtitle={row.original.nameAr ?? undefined}
        subtitleDir="rtl"
        fallback={
          row.original.parentId === null ? (
            <FolderOpen className="text-primary size-5" />
          ) : (
            <FolderTree className="text-muted-foreground size-5" />
          )
        }
      />
    ),
  },
  {
    accessorKey: 'slug',
    header: 'Slug',
    cell: ({ row }) => (
      <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 text-xs">
        {row.original.slug}
      </code>
    ),
  },
  {
    accessorKey: 'parentNameEn',
    header: 'الفئة الأم',
    cell: ({ row }) =>
      row.original.parentNameEn ? (
        <span className="text-sm">{row.original.parentNameEn}</span>
      ) : (
        <Badge variant="outline" className="text-xs font-normal">
          رئيسية
        </Badge>
      ),
  },
  {
    accessorKey: 'status',
    header: 'الحالة',
    cell: ({ row }) => (
      <CategoryStatusToggle
        categoryId={row.original.id}
        status={row.original.status}
      />
    ),
  },
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => <CategoryCellAction category={row.original} />,
  },
];
