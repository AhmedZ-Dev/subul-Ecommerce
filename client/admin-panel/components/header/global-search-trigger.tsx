'use client';

import { useKBar } from 'kbar';
import { SearchIcon } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';

interface GlobalSearchTriggerProps {
  className?: string;
}

export function GlobalSearchTrigger({ className }: GlobalSearchTriggerProps) {
  const { query } = useKBar();

  return (
    <Button
      type="button"
      variant="outline"
      onClick={() => query.toggle()}
      className={cn(
        'text-muted-foreground h-8 w-full max-w-sm justify-start gap-2 px-2.5 font-normal',
        className,
      )}
      aria-label={messages.header.searchPlaceholder}
    >
      <SearchIcon className="size-4 shrink-0 opacity-60" />
      <span className="flex-1 truncate text-start text-sm">
        {messages.header.searchPlaceholder}
      </span>
      <kbd className="bg-muted text-muted-foreground pointer-events-none hidden h-5 items-center gap-0.5 rounded border px-1.5 font-mono text-[10px] font-medium sm:inline-flex">
        <span className="text-xs">Ctrl</span>K
      </kbd>
    </Button>
  );
}
