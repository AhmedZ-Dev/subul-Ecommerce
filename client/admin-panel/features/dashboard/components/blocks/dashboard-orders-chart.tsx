'use client';

import { useState } from 'react';
import { Area, AreaChart, CartesianGrid, XAxis } from 'recharts';
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
  type ChartConfig,
} from '@/components/ui/chart';
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { formatCurrency, formatDate, formatNumber, messages } from '@/lib/messages.ar';
import type { OrdersByDayItemDto } from '../../types';

const m = messages.dashboard;

type ChartMetric = 'orders' | 'revenue';

const chartConfig = {
  orderCount: {
    label: m.orderCount,
    color: 'var(--chart-1)',
  },
  revenue: {
    label: m.revenue,
    color: 'var(--chart-2)',
  },
} satisfies ChartConfig;

interface DashboardOrdersChartProps {
  ordersByDay: OrdersByDayItemDto[];
  currency?: string;
}

export function DashboardOrdersChart({
  ordersByDay,
  currency = 'IQD',
}: DashboardOrdersChartProps) {
  const [metric, setMetric] = useState<ChartMetric>('orders');

  const chartData = ordersByDay.map((item) => ({
    date: item.date,
    orderCount: item.orderCount,
    revenue: item.revenue,
  }));

  const dataKey = metric === 'orders' ? 'orderCount' : 'revenue';
  const colorVar = metric === 'orders' ? 'var(--color-orderCount)' : 'var(--color-revenue)';
  const description = metric === 'orders' ? m.orderCount : m.revenue;

  return (
    <Card>
      <CardHeader>
        <CardTitle>{m.ordersLast30Days}</CardTitle>
        <CardDescription>
          {m.last30Days} · {description}
        </CardDescription>
        <CardAction>
          <ToggleGroup
            type="single"
            value={metric}
            onValueChange={(value) => {
              if (value === 'orders' || value === 'revenue') setMetric(value);
            }}
            variant="outline"
            size="sm"
          >
            <ToggleGroupItem value="orders">{m.chartMetricOrders}</ToggleGroupItem>
            <ToggleGroupItem value="revenue">{m.chartMetricRevenue}</ToggleGroupItem>
          </ToggleGroup>
        </CardAction>
      </CardHeader>
      <CardContent className="px-2 pt-4 sm:px-6 sm:pt-6">
        <ChartContainer config={chartConfig} className="aspect-auto h-[250px] w-full">
          <AreaChart data={chartData}>
            <defs>
              <linearGradient id="fillDashboardMetric" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor={colorVar} stopOpacity={0.8} />
                <stop offset="95%" stopColor={colorVar} stopOpacity={0.1} />
              </linearGradient>
            </defs>
            <CartesianGrid vertical={false} />
            <XAxis
              dataKey="date"
              tickLine={false}
              axisLine={false}
              tickMargin={8}
              minTickGap={32}
              tickFormatter={(value) =>
                formatDate(value, { month: 'short', day: 'numeric' })
              }
            />
            <ChartTooltip
              cursor={false}
              content={
                <ChartTooltipContent
                  labelFormatter={(value) =>
                    formatDate(value, { month: 'short', day: 'numeric' })
                  }
                  formatter={(value) => {
                    const num = Number(value);
                    if (metric === 'revenue') {
                      return [formatCurrency(num, currency), m.revenue];
                    }
                    return [formatNumber(num), m.orderCount];
                  }}
                  indicator="dot"
                />
              }
            />
            <Area
              dataKey={dataKey}
              type="natural"
              fill="url(#fillDashboardMetric)"
              stroke={colorVar}
            />
          </AreaChart>
        </ChartContainer>
      </CardContent>
    </Card>
  );
}
