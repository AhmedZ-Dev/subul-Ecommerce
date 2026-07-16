import type { ColumnDef } from '@tanstack/react-table';
import { EntityCell } from '@/components/ui/entity-cell';
import { formatCurrency, formatDate, messages } from '@/lib/messages.ar';
import { ClipboardList } from 'lucide-react';
import type { OrderListItem } from '../../../types';
import { OrderCellAction } from './cell-action';
import { OrderStatusBadge } from '../order-status-badge';
import { OrderPaymentStatusBadge } from '../order-payment-status-badge';
import { OrderFulfillmentStatusBadge } from '../order-fulfillment-status-badge';

const m = messages.order.listing;

export const orderColumns: ColumnDef<OrderListItem>[] = [
  {
    accessorKey: 'orderNumber',
    header: m.columnOrderNumber,
    cell: ({ row }) => (
      <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 text-xs" dir="ltr">
        {row.original.orderNumber}
      </code>
    ),
  },
  {
    id: 'customer',
    header: m.columnCustomer,
    cell: ({ row }) => {
      const { shippingFirstName, shippingPhone } = row.original;
      const title = shippingFirstName ?? m.noCustomer;
      return (
        <EntityCell
          title={title}
          subtitle={shippingPhone ?? undefined}
          fallback={<ClipboardList className="text-primary size-5" />}
        />
      );
    },
  },
  {
    id: 'location',
    header: m.columnLocation,
    cell: ({ row }) => {
      const { shippingCity, shippingGovernorate } = row.original;
      const parts = [shippingCity, shippingGovernorate].filter(Boolean);
      return (
        <span className="text-sm">
          {parts.length > 0 ? parts.join('، ') : messages.order.view.empty}
        </span>
      );
    },
  },
  {
    accessorKey: 'total',
    header: m.columnTotal,
    cell: ({ row }) => (
      <span dir="ltr" className="font-medium tabular-nums">
        {formatCurrency(row.original.total, row.original.currency)}
      </span>
    ),
  },
  {
    accessorKey: 'status',
    header: m.columnStatus,
    cell: ({ row }) => <OrderStatusBadge status={row.original.status} />,
  },
  {
    accessorKey: 'paymentStatus',
    header: m.columnPaymentStatus,
    cell: ({ row }) => <OrderPaymentStatusBadge status={row.original.paymentStatus} />,
  },
  {
    accessorKey: 'fulfillmentStatus',
    header: m.columnFulfillmentStatus,
    cell: ({ row }) => (
      <OrderFulfillmentStatusBadge status={row.original.fulfillmentStatus} />
    ),
  },
  {
    accessorKey: 'createdAt',
    header: m.columnCreatedAt,
    cell: ({ row }) => (
      <span dir="ltr" className="text-muted-foreground text-sm tabular-nums">
        {formatDate(row.original.createdAt)}
      </span>
    ),
  },
  {
    id: 'actions',
    header: m.columnActions,
    cell: ({ row }) => <OrderCellAction order={row.original} />,
  },
];
