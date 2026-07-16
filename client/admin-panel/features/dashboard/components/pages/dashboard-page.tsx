import { PageContainer } from '@/components/layout/page-container';
import { PageHeader } from '@/components/layout/page-header';
import { messages } from '@/lib/messages.ar';
import type { DashboardStatsDto } from '../../types';
import { DashboardKpiCards } from '../blocks/dashboard-kpi-cards';
import { DashboardLowStock } from '../blocks/dashboard-low-stock';
import { DashboardOrdersChart } from '../blocks/dashboard-orders-chart';
import { DashboardRecentOrders } from '../blocks/dashboard-recent-orders';
import { DashboardStatusOverview } from '../blocks/dashboard-status-overview';
import { DashboardTopSellers } from '../blocks/dashboard-top-sellers';

const m = messages.dashboard;

interface DashboardPageProps {
  stats: DashboardStatsDto | null;
}

export function DashboardPage({ stats }: DashboardPageProps) {
  if (!stats) {
    return (
      <PageContainer>
        <PageHeader title={m.title} description={m.description} />
        <p className="text-muted-foreground py-12 text-center text-sm">{m.loadError}</p>
      </PageContainer>
    );
  }

  const currency = stats.topSellingProducts[0]?.currency
    ?? stats.recentOrders[0]?.currency
    ?? 'IQD';

  return (
    <PageContainer>
      <div className="flex flex-col gap-6 pb-6">
        <PageHeader title={m.title} description={m.description} />
        <DashboardKpiCards stats={stats} />
        <DashboardStatusOverview stats={stats} />
        <DashboardOrdersChart ordersByDay={stats.ordersByDay} currency={currency} />
        <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
          <DashboardRecentOrders orders={stats.recentOrders} />
          <DashboardLowStock products={stats.lowStockProducts} />
        </div>
        <DashboardTopSellers products={stats.topSellingProducts} />
      </div>
    </PageContainer>
  );
}
