import type { ColumnDef } from '@tanstack/react-table';
import { EntityCell } from '@/components/ui/entity-cell';
import { formatCurrency } from '@/lib/messages.ar';
import { Package } from 'lucide-react';
import type { ProductListItem } from '../../../types';
import { ProductCellAction } from './cell-action';
import { ProductStatusBadge } from '../product-status-badge';

export const productColumns: ColumnDef<ProductListItem>[] = [
  {
    accessorKey: 'nameEn',
    header: 'الاسم',
    cell: ({ row }) => (
      <EntityCell
        title={row.original.nameEn}
        subtitle={row.original.nameAr ?? undefined}
        fallback={<Package className="text-primary size-5" />}
      />
    ),
  },
  {
    accessorKey: 'sku',
    header: 'SKU',
    cell: ({ row }) =>
      row.original.sku ? (
        <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 text-xs">
          {row.original.sku}
        </code>
      ) : (
        <span className="text-muted-foreground text-sm">—</span>
      ),
  },
  {
    accessorKey: 'price',
    header: 'السعر',
    cell: ({ row }) => (
      <span dir="ltr" className="font-medium tabular-nums">
        {formatCurrency(row.original.price, row.original.currency)}
      </span>
    ),
  },
  {
    accessorKey: 'stockQuantity',
    header: 'المخزون',
    cell: ({ row }) => (
      <span dir="ltr" className="tabular-nums">
        {row.original.stockQuantity}
      </span>
    ),
  },
  {
    accessorKey: 'status',
    header: 'الحالة',
    cell: ({ row }) => <ProductStatusBadge status={row.original.status} />,
  },
  {
    id: 'actions',
    header: 'الإجراءات',
    cell: ({ row }) => <ProductCellAction product={row.original} />,
  },
];
