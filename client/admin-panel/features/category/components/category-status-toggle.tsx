'use client';

import { useEffect, useState } from 'react';
import { Loader2 } from 'lucide-react';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import type { CategoryStatus } from '../types';
import type { ChangeCategoryStatusResult } from '../api/category.api';
import { useChangeCategoryStatus } from '../hooks/useCategoryMutations';
import { CategoryStatusBadge } from './category-status-badge';

interface CategoryStatusToggleProps {
  categoryId: number;
  status: CategoryStatus;
  className?: string;
  onStatusChange?: (result: ChangeCategoryStatusResult) => void;
}

export function CategoryStatusToggle({
  categoryId,
  status: initialStatus,
  className,
  onStatusChange,
}: CategoryStatusToggleProps) {
  const [status, setStatus] = useState(initialStatus);
  const { mutate, isPending } = useChangeCategoryStatus();

  useEffect(() => {
    setStatus(initialStatus);
  }, [initialStatus]);

  const nextIsActive = status !== 'active';
  const tooltip = nextIsActive
    ? messages.category.status.toggleActive
    : messages.category.status.toggleInactive;

  function handleToggle() {
    mutate(
      { id: categoryId, isActive: nextIsActive },
      {
        onSuccess: (result) => {
          setStatus(result.status);
          onStatusChange?.(result);
        },
      },
    );
  }

  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <button
          type="button"
          onClick={handleToggle}
          disabled={isPending}
          aria-label={tooltip}
          className={cn(
            'inline-flex items-center rounded-full transition-opacity focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none disabled:cursor-wait disabled:opacity-60',
            className,
          )}
        >
          <CategoryStatusBadge status={status} />
          {isPending && (
            <Loader2 className="text-muted-foreground ms-1.5 size-3.5 animate-spin" />
          )}
        </button>
      </TooltipTrigger>
      <TooltipContent side="top">{tooltip}</TooltipContent>
    </Tooltip>
  );
}
