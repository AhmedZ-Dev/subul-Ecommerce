import type { ColumnDef } from '@tanstack/react-table';
import { EntityCell } from '@/components/ui/entity-cell';
import { resolveAssetUrl } from '@/lib/asset-url';
import { Tag } from 'lucide-react';
import type { BrandListItem } from '../../../types';
import { BrandCellAction } from './cell-action';
import { BrandStatusBadge } from '../brand-status-badge';

export const brandColumns: ColumnDef<BrandListItem>[] = [
  {
    accessorKey: 'name',
    header: 'الاسم',
    cell: ({ row }) => (
      <EntityCell
        title={row.original.name}
        thumbnailUrl={resolveAssetUrl(row.original.logoUrl)}
        thumbnailAlt={row.original.name}
        fallback={<Tag className="text-primary size-5" />}
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
    accessorKey: 'status',
    header: 'الحالة',
    cell: ({ row }) => <BrandStatusBadge status={row.original.status} />,
  },
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => <BrandCellAction brand={row.original} />,
  },
];
