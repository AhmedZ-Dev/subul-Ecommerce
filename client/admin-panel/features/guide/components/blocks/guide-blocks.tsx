import {
  InfoIcon,
  LightbulbIcon,
  TriangleAlertIcon,
  OctagonAlertIcon,
  ChevronLeftIcon,
} from 'lucide-react';

import { cn } from '@/lib/utils';
import type { GuideBlock, GuideTone } from '../../types';

const toneStyles: Record<
  GuideTone,
  { wrapper: string; icon: typeof InfoIcon; iconColor: string }
> = {
  info: {
    wrapper: 'border-sky-200 bg-sky-50/70 dark:border-sky-900 dark:bg-sky-950/30',
    icon: InfoIcon,
    iconColor: 'text-sky-600 dark:text-sky-400',
  },
  tip: {
    wrapper:
      'border-emerald-200 bg-emerald-50/70 dark:border-emerald-900 dark:bg-emerald-950/30',
    icon: LightbulbIcon,
    iconColor: 'text-emerald-600 dark:text-emerald-400',
  },
  warning: {
    wrapper:
      'border-amber-200 bg-amber-50/70 dark:border-amber-900 dark:bg-amber-950/30',
    icon: TriangleAlertIcon,
    iconColor: 'text-amber-600 dark:text-amber-400',
  },
  danger: {
    wrapper: 'border-red-200 bg-red-50/70 dark:border-red-900 dark:bg-red-950/30',
    icon: OctagonAlertIcon,
    iconColor: 'text-red-600 dark:text-red-400',
  },
};

export function GuideBlockRenderer({ block }: { block: GuideBlock }) {
  switch (block.kind) {
    case 'text':
      return (
        <p className="text-muted-foreground text-sm leading-7">{block.text}</p>
      );

    case 'list':
      return (
        <div className="flex flex-col gap-2">
          {block.title && (
            <p className="text-sm font-medium">{block.title}</p>
          )}
          <ul className="flex flex-col gap-1.5">
            {block.items.map((item) => (
              <li
                key={item}
                className="text-muted-foreground flex gap-2.5 text-sm leading-7"
              >
                <span
                  className="bg-muted-foreground/40 mt-3 size-1.5 shrink-0 rounded-full"
                  aria-hidden
                />
                <span className="min-w-0">{item}</span>
              </li>
            ))}
          </ul>
        </div>
      );

    case 'steps':
      return (
        <div className="flex flex-col gap-2">
          {block.title && <p className="text-sm font-medium">{block.title}</p>}
          <ol className="flex flex-col gap-2.5">
            {block.items.map((item, index) => (
              <li key={item} className="flex gap-3">
                <span className="bg-primary/10 text-primary flex size-6 shrink-0 items-center justify-center rounded-full text-xs font-semibold tabular-nums">
                  {index + 1}
                </span>
                <span className="text-muted-foreground min-w-0 pt-0.5 text-sm leading-7">
                  {item}
                </span>
              </li>
            ))}
          </ol>
        </div>
      );

    case 'fields':
      return (
        <div className="flex flex-col gap-2">
          {block.title && <p className="text-sm font-medium">{block.title}</p>}
          <dl className="divide-border divide-y rounded-lg border">
            {block.items.map((field) => (
              <div
                key={field.name}
                className="grid gap-1 p-3 sm:grid-cols-[minmax(0,14rem)_1fr] sm:gap-4"
              >
                <dt className="flex items-start gap-1.5 text-sm font-medium">
                  <span className="min-w-0">{field.name}</span>
                  {field.required && (
                    <span
                      className="text-destructive shrink-0"
                      title="حقل مطلوب"
                      aria-label="حقل مطلوب"
                    >
                      *
                    </span>
                  )}
                </dt>
                <dd className="text-muted-foreground text-sm leading-7">
                  {field.description}
                </dd>
              </div>
            ))}
          </dl>
        </div>
      );

    case 'callout': {
      const { wrapper, icon: Icon, iconColor } = toneStyles[block.tone];
      return (
        <div className={cn('flex gap-3 rounded-lg border p-3.5', wrapper)}>
          <Icon className={cn('mt-0.5 size-4.5 shrink-0', iconColor)} aria-hidden />
          <div className="flex min-w-0 flex-col gap-1">
            <p className="text-sm font-semibold">{block.title}</p>
            <p className="text-muted-foreground text-sm leading-7">{block.text}</p>
          </div>
        </div>
      );
    }

    case 'path':
      return (
        <div className="bg-muted/50 flex flex-wrap items-center gap-1.5 rounded-lg border px-3 py-2 text-sm">
          <span className="text-muted-foreground shrink-0">{block.label}:</span>
          {block.segments.map((segment, index) => (
            <span key={segment} className="flex items-center gap-1.5">
              {index > 0 && (
                <ChevronLeftIcon
                  className="text-muted-foreground/60 size-3.5"
                  aria-hidden
                />
              )}
              <span className="font-medium">{segment}</span>
            </span>
          ))}
        </div>
      );
  }
}
