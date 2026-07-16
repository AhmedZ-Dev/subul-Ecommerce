import type { ColumnDef } from '@tanstack/react-table';
import { messages } from '@/lib/messages.ar';
import type { PaymentMethodListItem } from '../../../types';
import { PaymentMethodCellAction } from './cell-action';
import { PaymentMethodStatusToggle } from '../payment-method-status-toggle';

const typeLabels = messages.paymentMethod.type;

function getDisplayLabel(item: PaymentMethodListItem): string {
  return item.labelAr ?? item.labelEn ?? item.name;
}

export const paymentMethodColumns: ColumnDef<PaymentMethodListItem>[] = [
  {
    accessorKey: 'name',
    header: messages.paymentMethod.listing.columnName,
    cell: ({ row }) => (
      <div className="space-y-0.5">
        <p className="font-medium">{getDisplayLabel(row.original)}</p>
        <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 text-xs" dir="ltr">
          {row.original.name}
        </code>
      </div>
    ),
  },
  {
    accessorKey: 'type',
    header: messages.paymentMethod.listing.columnType,
    cell: ({ row }) =>
      row.original.type ? (
        <span>{typeLabels[row.original.type]}</span>
      ) : (
        <span className="text-muted-foreground">{messages.paymentMethod.view.empty}</span>
      ),
  },
  {
    accessorKey: 'gateway',
    header: messages.paymentMethod.listing.columnGateway,
    cell: ({ row }) =>
      row.original.gateway ? (
        <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 text-xs" dir="ltr">
          {row.original.gateway}
        </code>
      ) : (
        <span className="text-muted-foreground">{messages.paymentMethod.view.empty}</span>
      ),
  },
  {
    accessorKey: 'sortOrder',
    header: messages.paymentMethod.listing.columnSortOrder,
    cell: ({ row }) => row.original.sortOrder,
  },
  {
    accessorKey: 'status',
    header: messages.paymentMethod.listing.columnStatus,
    cell: ({ row }) => (
      <PaymentMethodStatusToggle
        paymentMethodId={row.original.id}
        status={row.original.status}
      />
    ),
  },
  {
    id: 'actions',
    header: messages.paymentMethod.listing.columnActions,
    cell: ({ row }) => <PaymentMethodCellAction paymentMethod={row.original} />,
  },
];
