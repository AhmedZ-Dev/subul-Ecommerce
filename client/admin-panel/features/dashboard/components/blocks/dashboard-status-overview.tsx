import Link from 'next/link';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { formatNumber, messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import type { DashboardStatsDto } from '../../types';

const m = messages.dashboard;

interface DashboardStatusOverviewProps {
  stats: DashboardStatsDto;
}

type OverviewItem = {
  key: string;
  label: string;
  value: number;
  href?: string;
  barClassName: string;
};

function OverviewSection({
  title,
  description,
  items,
  total,
}: {
  title: string;
  description: string;
  items: OverviewItem[];
  total: number;
}) {
  return (
    <Card className="shadow-xs">
      <CardHeader>
        <CardTitle className="text-base">{title}</CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
        {items.map((item) => {
          const percent = total > 0 ? Math.round((item.value / total) * 100) : 0;
          const content = (
            <>
              <div className="flex items-center justify-between gap-3 text-sm">
                <span className="text-muted-foreground">{item.label}</span>
                <span className="font-medium tabular-nums">
                  {formatNumber(item.value)}
                  <span className="text-muted-foreground ms-2 text-xs font-normal">
                    {formatNumber(percent)}%
                  </span>
                </span>
              </div>
              <div className="bg-muted h-1.5 overflow-hidden rounded-full">
                <div
                  className={cn('h-full rounded-full transition-all', item.barClassName)}
                  style={{ width: `${percent}%` }}
                />
              </div>
            </>
          );

          if (item.href && item.value > 0) {
            return (
              <Link
                key={item.key}
                href={item.href}
                className="flex flex-col gap-1.5 rounded-md transition-colors hover:opacity-90"
              >
                {content}
              </Link>
            );
          }

          return (
            <div key={item.key} className="flex flex-col gap-1.5">
              {content}
            </div>
          );
        })}
      </CardContent>
    </Card>
  );
}

export function DashboardStatusOverview({ stats }: DashboardStatusOverviewProps) {
  const { orders, products } = stats;
  const statusTotal =
    orders.pending +
    orders.processing +
    orders.shipped +
    orders.delivered +
    orders.cancelled;
  const paymentTotal = orders.paid + orders.unpaid;
  const inventoryTotal = products.active + products.outOfStock;

  return (
    <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
      <OverviewSection
        title={m.orderStatusOverview}
        description={m.totalOrders}
        total={statusTotal || orders.total}
        items={[
          {
            key: 'pending',
            label: m.statusPending,
            value: orders.pending,
            href: '/orders?status=pending',
            barClassName: 'bg-amber-500',
          },
          {
            key: 'processing',
            label: m.statusProcessing,
            value: orders.processing,
            href: '/orders?status=processing',
            barClassName: 'bg-indigo-500',
          },
          {
            key: 'shipped',
            label: m.statusShipped,
            value: orders.shipped,
            href: '/orders?status=shipped',
            barClassName: 'bg-sky-500',
          },
          {
            key: 'delivered',
            label: m.statusDelivered,
            value: orders.delivered,
            href: '/orders?status=delivered',
            barClassName: 'bg-emerald-500',
          },
          {
            key: 'cancelled',
            label: m.statusCancelled,
            value: orders.cancelled,
            href: '/orders?status=cancelled',
            barClassName: 'bg-muted-foreground/50',
          },
        ]}
      />

      <OverviewSection
        title={m.paymentOverview}
        description={m.payment}
        total={paymentTotal || orders.total}
        items={[
          {
            key: 'paid',
            label: m.paymentPaid,
            value: orders.paid,
            href: '/orders?paymentStatus=paid',
            barClassName: 'bg-emerald-500',
          },
          {
            key: 'unpaid',
            label: m.paymentUnpaid,
            value: orders.unpaid,
            href: '/orders?paymentStatus=pending',
            barClassName: 'bg-amber-500',
          },
        ]}
      />

      <OverviewSection
        title={m.inventoryOverview}
        description={m.activeProducts}
        total={inventoryTotal || products.total}
        items={[
          {
            key: 'active',
            label: m.activeProducts,
            value: products.active,
            href: '/products',
            barClassName: 'bg-emerald-500',
          },
          {
            key: 'low-stock',
            label: m.lowStockAlert,
            value: products.lowStock,
            href: '/products',
            barClassName: 'bg-amber-500',
          },
          {
            key: 'out-of-stock',
            label: m.outOfStockCount,
            value: products.outOfStock,
            href: '/products',
            barClassName: 'bg-red-500',
          },
        ]}
      />
    </div>
  );
}
