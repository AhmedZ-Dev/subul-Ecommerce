'use client';

import Link from 'next/link';
import { Eye } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { messages } from '@/lib/messages.ar';
import type { OrderListItem } from '../../../types';

interface OrderCellActionProps {
  order: OrderListItem;
}

export function OrderCellAction({ order }: OrderCellActionProps) {
  return (
    <div className="flex items-center justify-start gap-1">
      <Tooltip>
        <TooltipTrigger asChild>
          <span className="inline-flex">
            <Button
              asChild
              variant="outline"
              size="icon-sm"
              id={`order-view-${order.id}`}
              aria-label={messages.common.view}
            >
              <Link href={`/orders/${order.id}/view`}>
                <Eye />
                <span className="sr-only">{messages.common.view}</span>
              </Link>
            </Button>
          </span>
        </TooltipTrigger>
        <TooltipContent side="top">{messages.common.view}</TooltipContent>
      </Tooltip>
    </div>
  );
}
