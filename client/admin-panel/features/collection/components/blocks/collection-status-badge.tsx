import { Badge } from '@/components/ui/badge';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { COLLECTION_STATUS_COLORS, COLLECTION_STATUS_DOT_COLORS } from '../../constants';
import type { CollectionStatus } from '../../types';

const statusLabels = messages.collection.status;

interface CollectionStatusBadgeProps {
  status: CollectionStatus;
  className?: string;
}

export function CollectionStatusBadge({ status, className }: CollectionStatusBadgeProps) {
  const label = statusLabels[status] ?? status;

  return (
    <Badge variant="outline" className={cn(COLLECTION_STATUS_COLORS[status], className)}>
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
            COLLECTION_STATUS_DOT_COLORS[status],
            status === 'active' && 'motion-reduce:animate-none animate-pulse',
          )}
        />
      </span>
      {label}
    </Badge>
  );
}
