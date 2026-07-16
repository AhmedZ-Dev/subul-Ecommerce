import { Badge } from '@/components/ui/badge';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import {
  ORDER_PAYMENT_STATUS_COLORS,
  ORDER_PAYMENT_STATUS_DOT_COLORS,
} from '../../constants';
import type { OrderPaymentStatus } from '../../types';

const statusLabels = messages.order.paymentStatus;

interface OrderPaymentStatusBadgeProps {
  status: OrderPaymentStatus;
  className?: string;
}

export function OrderPaymentStatusBadge({ status, className }: OrderPaymentStatusBadgeProps) {
  const label = statusLabels[status] ?? status;

  return (
    <Badge variant="outline" className={cn(ORDER_PAYMENT_STATUS_COLORS[status], className)}>
      <span
        className="relative flex size-1.5 shrink-0"
        data-icon="inline-start"
        aria-hidden="true"
      >
        {status === 'paid' && (
          <span className="absolute inline-flex size-full animate-ping rounded-full bg-green-400 opacity-60 motion-reduce:animate-none dark:bg-green-500 dark:opacity-50" />
        )}
        <span
          className={cn(
            'relative size-1.5 rounded-full',
            ORDER_PAYMENT_STATUS_DOT_COLORS[status],
            status === 'paid' && 'motion-reduce:animate-none animate-pulse',
          )}
        />
      </span>
      {label}
    </Badge>
  );
}
