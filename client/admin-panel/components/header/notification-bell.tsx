'use client';

import { BellIcon } from 'lucide-react';

import { Button } from '@/components/ui/button';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { messages } from '@/lib/messages.ar';

export function NotificationBell() {
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <span className="inline-flex">
          <Button
            type="button"
            variant="ghost"
            size="icon"
            disabled
            aria-label={messages.header.notifications}
            className="relative"
          >
            <BellIcon className="size-4" />
          </Button>
        </span>
      </TooltipTrigger>
      <TooltipContent side="bottom">
        {messages.header.notificationsComingSoon}
      </TooltipContent>
    </Tooltip>
  );
}
