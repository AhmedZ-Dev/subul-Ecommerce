'use client';

import { SearchIcon, XIcon } from 'lucide-react';

import { Input } from '@/components/ui/input';
import { cn } from '@/lib/utils';
import type { GuideSection } from '../../types';

interface GuideTocProps {
  sections: GuideSection[];
  activeId: string | null;
  query: string;
  onQueryChange: (value: string) => void;
  onNavigate: (id: string) => void;
  className?: string;
}

/**
 * Table of contents with a filter box, so the reader can jump either by
 * browsing the outline or by typing what they are trying to do.
 */
export function GuideToc({
  sections,
  activeId,
  query,
  onQueryChange,
  onNavigate,
  className,
}: GuideTocProps) {
  const hasResults = sections.length > 0;

  return (
    <nav className={cn('flex flex-col gap-3', className)} aria-label="محتويات الدليل">
      <div className="relative">
        <SearchIcon
          className="text-muted-foreground pointer-events-none absolute start-3 top-1/2 size-4 -translate-y-1/2"
          aria-hidden
        />
        <Input
          value={query}
          onChange={(event) => onQueryChange(event.target.value)}
          placeholder="ابحث عن موضوع…"
          className="h-9 ps-9 pe-9"
          aria-label="بحث في محتويات الدليل"
        />
        {query && (
          <button
            type="button"
            onClick={() => onQueryChange('')}
            className="text-muted-foreground hover:text-foreground absolute end-2 top-1/2 -translate-y-1/2 rounded-sm p-1 transition-colors"
            aria-label="مسح البحث"
          >
            <XIcon className="size-3.5" />
          </button>
        )}
      </div>

      {!hasResults ? (
        <p className="text-muted-foreground px-1 py-6 text-center text-sm">
          لا يوجد موضوع مطابق.
        </p>
      ) : (
        <ul className="flex flex-col gap-1">
          {sections.map((section) => {
            const isSectionActive =
              activeId === section.id ||
              section.subsections.some((sub) => sub.id === activeId);

            return (
              <li key={section.id} className="flex flex-col">
                <button
                  type="button"
                  onClick={() => onNavigate(section.id)}
                  className={cn(
                    'flex items-center gap-2 rounded-md px-2 py-1.5 text-start text-sm font-medium transition-colors',
                    isSectionActive
                      ? 'bg-primary/10 text-primary'
                      : 'text-foreground hover:bg-muted',
                  )}
                >
                  <section.icon className="size-4 shrink-0" aria-hidden />
                  <span className="min-w-0 truncate">{section.title}</span>
                </button>

                {isSectionActive && section.subsections.length > 0 && (
                  <ul className="border-border mt-1 flex flex-col gap-0.5 border-s ps-2 ms-4">
                    {section.subsections.map((sub) => (
                      <li key={sub.id}>
                        <button
                          type="button"
                          onClick={() => onNavigate(sub.id)}
                          className={cn(
                            'w-full rounded-md px-2 py-1.5 text-start text-xs leading-5 transition-colors',
                            activeId === sub.id
                              ? 'text-primary font-medium'
                              : 'text-muted-foreground hover:text-foreground hover:bg-muted',
                          )}
                        >
                          {sub.title}
                        </button>
                      </li>
                    ))}
                  </ul>
                )}
              </li>
            );
          })}
        </ul>
      )}
    </nav>
  );
}
