import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';
import {
  CATEGORY_STATUS_COLORS,
  CATEGORY_STATUS_DOT_COLORS,
  CATEGORY_STATUS_OPTIONS,
} from '../constants';
import type { CategoryStatus } from '../types';

interface CategoryStatusBadgeProps {
  status: CategoryStatus;
  className?: string;
}

export function CategoryStatusBadge({ status, className }: CategoryStatusBadgeProps) {
  const label =
    CATEGORY_STATUS_OPTIONS.find((option) => option.value === status)?.label ??
    status;

  return (
    <Badge variant="outline" className={cn(CATEGORY_STATUS_COLORS[status], className)}>
      <span
        className="relative flex size-1.5 shrink-0"
        data-icon="inline-start"
        aria-hidden="true"
      >
        {status === 'active' && (
          <span className="absolute inline-flex size-full animate-ping rounded-full bg-green-400 opacity-60 motion-reduce:animate-none" />
        )}
        <span
          className={cn(
            'relative size-1.5 rounded-full',
            CATEGORY_STATUS_DOT_COLORS[status],
            status === 'active' && 'motion-reduce:animate-none animate-pulse',
          )}
        />
      </span>
      {label}
    </Badge>
  );
}
