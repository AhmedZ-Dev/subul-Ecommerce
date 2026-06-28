'use client';

import { usePathname } from 'next/navigation';

import { BreadcrumbNav } from '@/components/header/breadcrumb-nav';
import { getBreadcrumbs } from '@/lib/breadcrumbs';
import { cn } from '@/lib/utils';

interface HeaderBreadcrumbProps {
  className?: string;
}

export function HeaderBreadcrumb({ className }: HeaderBreadcrumbProps) {
  const pathname = usePathname();
  const items = getBreadcrumbs(pathname);
  const mobileItems = items.length > 0 ? [items[items.length - 1]!] : [];

  return (
    <>
      <BreadcrumbNav
        items={mobileItems}
        className={cn('min-w-0 flex-1 sm:hidden', className)}
      />
      <BreadcrumbNav
        items={items}
        className={cn('min-w-0 hidden flex-1 sm:flex', className)}
      />
    </>
  );
}
