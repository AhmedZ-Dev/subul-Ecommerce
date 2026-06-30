'use client';

import { ColumnDef } from '@tanstack/react-table';
import { Badge } from '@/components/ui/badge';
import { Check, X } from 'lucide-react';
import { CellAction } from './cell-action';
import type { AttributeGroupListItem } from '../../../types';
import { messages } from '@/lib/messages.ar';

const t = messages.attributeGroup.fields;

export const attributeGroupColumns: ColumnDef<AttributeGroupListItem>[] = [
  {
    accessorKey: 'nameEn',
    header: t.nameEn,
  },
  {
    accessorKey: 'nameAr',
    header: t.nameAr,
    cell: ({ row }) => row.original.nameAr || '-',
  },
  {
    accessorKey: 'slug',
    header: t.slug,
  },
  {
    accessorKey: 'isFilterable',
    header: t.isFilterable,
    cell: ({ row }) => {
      return row.original.isFilterable ? (
        <Check className="h-4 w-4 text-green-500" />
      ) : (
        <X className="h-4 w-4 text-red-500" />
      );
    },
  },
  {
    accessorKey: 'attributeCount',
    header: t.attributeCount,
    cell: ({ row }) => (
      <Badge variant="secondary">{row.original.attributeCount}</Badge>
    ),
  },
  {
    accessorKey: 'sortOrder',
    header: t.sortOrder,
  },
  {
    id: 'actions',
    cell: ({ row }) => <CellAction data={row.original} />,
  },
];
