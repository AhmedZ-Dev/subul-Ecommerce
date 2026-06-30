import type { ColumnDef } from '@tanstack/react-table';
import { EntityCell } from '@/components/ui/entity-cell';
import { FolderOpen } from 'lucide-react';
import type { CollectionListItem } from '../../../types';
import { CollectionCellAction } from './cell-action';
import { CollectionStatusBadge } from '../collection-status-badge';
import { CollectionTypeBadge } from '../collection-type-badge';

export const collectionColumns: ColumnDef<CollectionListItem>[] = [
  {
    accessorKey: 'nameEn',
    header: 'الاسم',
    cell: ({ row }) => (
      <EntityCell
        title={row.original.nameEn}
        subtitle={row.original.nameAr ?? undefined}
        fallback={<FolderOpen className="text-primary size-5" />}
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
    accessorKey: 'collectionType',
    header: 'النوع',
    cell: ({ row }) => <CollectionTypeBadge type={row.original.collectionType} />,
  },
  {
    accessorKey: 'status',
    header: 'الحالة',
    cell: ({ row }) => <CollectionStatusBadge status={row.original.status} />,
  },
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => <CollectionCellAction collection={row.original} />,
  },
];
