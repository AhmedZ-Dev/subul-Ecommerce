import { messages } from '@/lib/messages.ar';

export type BreadcrumbItem = {
  label: string;
  href?: string;
};

const home: BreadcrumbItem = {
  label: messages.breadcrumb.home,
  href: '/dashboard',
};

export function getBreadcrumbs(pathname: string): BreadcrumbItem[] {
  if (pathname === '/dashboard') {
    return [home, { label: messages.nav.dashboard }];
  }

  if (pathname === '/categories') {
    return [home, { label: messages.nav.categories }];
  }

  if (pathname === '/categories/new') {
    return [
      home,
      { label: messages.nav.categories, href: '/categories' },
      { label: messages.breadcrumb.newCategory },
    ];
  }

  const editMatch = pathname.match(/^\/categories\/(\d+)\/edit$/);
  if (editMatch) {
    return [
      home,
      { label: messages.nav.categories, href: '/categories' },
      { label: messages.breadcrumb.editCategory },
    ];
  }

  const viewMatch = pathname.match(/^\/categories\/(\d+)\/view$/);
  if (viewMatch) {
    return [
      home,
      { label: messages.nav.categories, href: '/categories' },
      { label: messages.breadcrumb.viewCategory },
    ];
  }

  return [home, { label: messages.breadcrumb.currentPage }];
}
