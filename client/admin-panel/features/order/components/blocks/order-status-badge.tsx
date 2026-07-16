import { Badge } from '@/components/ui/badge';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { ORDER_STATUS_COLORS, ORDER_STATUS_DOT_COLORS } from '../../constants';
import type { OrderStatus } from '../../types';

const statusLabels = messages.order.status;

interface OrderStatusBadgeProps {
  status: OrderStatus;
  className?: string;
}

export function OrderStatusBadge({ status, className }: OrderStatusBadgeProps) {
  const label = statusLabels[status] ?? status;
  const isActive = status === 'processing' || status === 'out_for_delivery';

  return (
    <Badge variant="outline" className={cn(ORDER_STATUS_COLORS[status], className)}>
      <span
        className="relative flex size-1.5 shrink-0"
        data-icon="inline-start"
        aria-hidden="true"
      >
        {isActive && (
          <span className="absolute inline-flex size-full animate-ping rounded-full bg-blue-400 opacity-60 motion-reduce:animate-none dark:bg-blue-500 dark:opacity-50" />
        )}
        <span
          className={cn(
            'relative size-1.5 rounded-full',
            ORDER_STATUS_DOT_COLORS[status],
            isActive && 'motion-reduce:animate-none animate-pulse',
          )}
        />
      </span>
      {label}
    </Badge>
  );
}
