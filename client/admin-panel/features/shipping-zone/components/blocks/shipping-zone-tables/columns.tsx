import type { ColumnDef } from '@tanstack/react-table';
import { EntityCell } from '@/components/ui/entity-cell';
import { MapPin } from 'lucide-react';
import { messages } from '@/lib/messages.ar';
import type { ShippingZoneListItem } from '../../../types';
import { ShippingZoneCellAction } from './cell-action';
import { ShippingZoneStatusBadge } from '../shipping-zone-status-badge';

const m = messages.shippingZone.listing;

export const shippingZoneColumns: ColumnDef<ShippingZoneListItem>[] = [
  {
    accessorKey: 'nameEn',
    header: m.columnName,
    cell: ({ row }) => (
      <EntityCell
        title={row.original.nameEn}
        subtitle={row.original.nameAr ?? undefined}
        fallback={<MapPin className="text-primary size-5" />}
      />
    ),
  },
  {
    accessorKey: 'governorates',
    header: m.columnGovernorates,
    cell: ({ row }) => (
      <span className="text-muted-foreground text-sm">
        {row.original.governorates.length}
      </span>
    ),
  },
  {
    accessorKey: 'shippingRateCount',
    header: m.columnRates,
    cell: ({ row }) => (
      <span className="text-muted-foreground text-sm">{row.original.shippingRateCount}</span>
    ),
  },
  {
    accessorKey: 'status',
    header: m.columnStatus,
    cell: ({ row }) => <ShippingZoneStatusBadge status={row.original.status} />,
  },
  {
    id: 'actions',
    header: m.columnActions,
    cell: ({ row }) => <ShippingZoneCellAction zone={row.original} />,
  },
];
