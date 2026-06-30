import { Badge } from '@/components/ui/badge';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { BRAND_STATUS_COLORS, BRAND_STATUS_DOT_COLORS } from '../../constants';
import type { BrandStatus } from '../../types';

const statusLabels = messages.brand.status;

interface BrandStatusBadgeProps {
  status: BrandStatus;
  className?: string;
}

export function BrandStatusBadge({ status, className }: BrandStatusBadgeProps) {
  const label = statusLabels[status] ?? status;

  return (
    <Badge variant="outline" className={cn(BRAND_STATUS_COLORS[status], className)}>
      <span
        className="relative flex size-1.5 shrink-0"
        data-icon="inline-start"
        aria-hidden="true"
      >
        {status === 'active' && (
          <span className="absolute inline-flex size-full animate-ping rounded-full bg-green-400 opacity-60 motion-reduce:animate-none dark:bg-green-500 dark:opacity-50" />
        )}
        <span
          className={cn(
            'relative size-1.5 rounded-full',
            BRAND_STATUS_DOT_COLORS[status],
            status === 'active' && 'motion-reduce:animate-none animate-pulse',
          )}
        />
      </span>
      {label}
    </Badge>
  );
}
