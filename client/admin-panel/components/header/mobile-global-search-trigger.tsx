'use client';

import { useKBar } from 'kbar';
import { SearchIcon } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { messages } from '@/lib/messages.ar';

export function MobileGlobalSearchTrigger() {
  const { query } = useKBar();

  return (
    <Button
      type="button"
      variant="ghost"
      size="icon"
      className="sm:hidden"
      onClick={() => query.toggle()}
      aria-label={messages.header.searchPlaceholder}
    >
      <SearchIcon className="size-4" />
    </Button>
  );
}
