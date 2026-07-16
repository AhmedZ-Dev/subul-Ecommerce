'use client';

import { Loader2 } from 'lucide-react';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import type { PaymentMethodStatus } from '../../types';
import { useChangePaymentMethodStatus } from '../../hooks/usePaymentMethodMutations';
import { PaymentMethodStatusBadge } from './payment-method-status-badge';

interface PaymentMethodStatusToggleProps {
  paymentMethodId: number;
  status: PaymentMethodStatus;
  className?: string;
}

export function PaymentMethodStatusToggle({
  paymentMethodId,
  status,
  className,
}: PaymentMethodStatusToggleProps) {
  const { mutate, isPending } = useChangePaymentMethodStatus();

  const nextIsActive = status !== 'active';
  const tooltip = nextIsActive
    ? messages.paymentMethod.status.toggleActive
    : messages.paymentMethod.status.toggleInactive;

  function handleToggle() {
    mutate({ id: paymentMethodId, isActive: nextIsActive });
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
          <PaymentMethodStatusBadge status={status} />
          {isPending && (
            <Loader2 className="text-muted-foreground ms-1.5 size-3.5 animate-spin" />
          )}
        </button>
      </TooltipTrigger>
      <TooltipContent side="top">{tooltip}</TooltipContent>
    </Tooltip>
  );
}
