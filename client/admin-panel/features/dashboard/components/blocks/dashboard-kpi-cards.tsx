import type { ReactNode } from 'react';
import { Badge } from '@/components/ui/badge';
import {
  Card,
  CardAction,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { formatCurrency, formatNumber, messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import {
  AlertTriangleIcon,
  DollarSignIcon,
  PackageIcon,
  ShoppingCartIcon,
  TrendingUpIcon,
  type LucideIcon,
} from 'lucide-react';
import type { DashboardStatsDto } from '../../types';

const m = messages.dashboard;

interface DashboardKpiCardsProps {
  stats: DashboardStatsDto;
}

type KpiCard = {
  key: string;
  label: string;
  value: string;
  footer: ReactNode;
  icon: LucideIcon;
  iconClassName: string;
  badge?: ReactNode;
};

export function DashboardKpiCards({ stats }: DashboardKpiCardsProps) {
  const topProduct = stats.topSellingProducts[0];
  const currency = topProduct?.currency ?? 'IQD';

  const cards: KpiCard[] = [
    {
      key: 'revenue',
      label: m.totalRevenue,
      value: formatCurrency(stats.orders.totalRevenue, currency),
      footer: (
        <>
          <span className="font-medium text-foreground">{m.revenueThisMonth}</span>
          <span>{formatCurrency(stats.orders.revenueThisMonth, currency)}</span>
        </>
      ),
      icon: DollarSignIcon,
      iconClassName: 'bg-primary/10 text-primary',
      badge:
        stats.orders.revenueToday > 0 ? (
          <Badge variant="outline" className="text-xs tabular-nums">
            {m.revenueToday}: {formatCurrency(stats.orders.revenueToday, currency)}
          </Badge>
        ) : undefined,
    },
    {
      key: 'orders',
      label: m.totalOrders,
      value: formatNumber(stats.orders.total),
      footer: (
        <>
          <span className="font-medium text-foreground">{m.pendingOrders}</span>
          <span>{formatNumber(stats.orders.pending)}</span>
        </>
      ),
      icon: ShoppingCartIcon,
      iconClassName: 'bg-blue-500/10 text-blue-600 dark:text-blue-400',
      badge:
        stats.orders.pending > 0 ? (
          <Badge variant="outline" className="border-blue-500/40 text-blue-700 dark:text-blue-400">
            {formatNumber(stats.orders.pending)} {m.pendingOrders}
          </Badge>
        ) : undefined,
    },
    {
      key: 'products',
      label: m.activeProducts,
      value: formatNumber(stats.products.active),
      footer: (
        <>
          <span className="flex items-center gap-1.5 font-medium text-foreground">
            {stats.products.lowStock > 0 && (
              <AlertTriangleIcon className="size-3.5 text-amber-500" />
            )}
            {m.lowStockAlert}
          </span>
          <span>{formatNumber(stats.products.lowStock)}</span>
        </>
      ),
      icon: PackageIcon,
      iconClassName: 'bg-emerald-500/10 text-emerald-600 dark:text-emerald-400',
      badge:
        stats.products.lowStock > 0 ? (
          <Badge
            variant="outline"
            className="border-amber-500/40 text-amber-700 dark:text-amber-400"
          >
            <AlertTriangleIcon />
            {formatNumber(stats.products.lowStock)}
          </Badge>
        ) : undefined,
    },
    {
      key: 'top-selling',
      label: m.topSelling,
      value: topProduct ? formatNumber(topProduct.totalSold) : '—',
      footer: (
        <>
          <span className="font-medium text-foreground">{m.bestSeller}</span>
          <span className="line-clamp-1">
            {topProduct ? (topProduct.nameAr ?? topProduct.nameEn) : m.noData}
          </span>
        </>
      ),
      icon: TrendingUpIcon,
      iconClassName: 'bg-violet-500/10 text-violet-600 dark:text-violet-400',
      badge: topProduct ? (
        <Badge variant="outline" className="text-xs">
          {m.unitsSold}
        </Badge>
      ) : undefined,
    },
  ];

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
      {cards.map((card) => {
        const Icon = card.icon;
        return (
          <Card key={card.key} className="overflow-hidden shadow-xs">
            <CardHeader>
              <div className="flex items-start justify-between gap-3">
                <CardDescription>{card.label}</CardDescription>
                <span
                  className={cn(
                    'flex size-9 shrink-0 items-center justify-center rounded-lg',
                    card.iconClassName,
                  )}
                >
                  <Icon className="size-4" />
                </span>
              </div>
              <CardTitle className="text-2xl font-semibold tabular-nums tracking-tight sm:text-3xl">
                {card.value}
              </CardTitle>
              {card.badge && <CardAction>{card.badge}</CardAction>}
            </CardHeader>
            <CardFooter className="text-muted-foreground flex flex-col items-start gap-0.5 text-sm">
              {card.footer}
            </CardFooter>
          </Card>
        );
      })}
    </div>
  );
}
