import { Badge } from '@/components/ui/badge';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import { COLLECTION_TYPE_COLORS } from '../../constants';
import type { CollectionType } from '../../types';

const typeLabels = {
  manual: messages.collection.form.typeManual,
  smart: messages.collection.form.typeSmart,
};

interface CollectionTypeBadgeProps {
  type: CollectionType;
  className?: string;
}

export function CollectionTypeBadge({ type, className }: CollectionTypeBadgeProps) {
  return (
    <Badge variant="outline" className={cn(COLLECTION_TYPE_COLORS[type], className)}>
      {typeLabels[type]}
    </Badge>
  );
}
