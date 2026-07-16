import Link from 'next/link';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { formatCurrency, formatDate, messages } from '@/lib/messages.ar';
import type { RecentOrderItemDto } from '../../types';

const m = messages.dashboard;
const orderStatusLabels = messages.order.status as Record<string, string>;
const paymentStatusLabels = messages.order.paymentStatus as Record<string, string>;

interface DashboardRecentOrdersProps {
  orders: RecentOrderItemDto[];
}

export function DashboardRecentOrders({ orders }: DashboardRecentOrdersProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{m.recentOrders}</CardTitle>
        <CardDescription>{m.totalOrders}</CardDescription>
        <CardAction>
          <Button variant="outline" size="sm" asChild>
            <Link href="/orders">{m.viewAllOrders}</Link>
          </Button>
        </CardAction>
      </CardHeader>
      <CardContent>
        {orders.length === 0 ? (
          <p className="text-muted-foreground py-8 text-center text-sm">{m.noData}</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>{m.orderNumber}</TableHead>
                <TableHead>{m.customer}</TableHead>
                <TableHead>{m.status}</TableHead>
                <TableHead>{m.payment}</TableHead>
                <TableHead>{m.total}</TableHead>
                <TableHead>{m.date}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {orders.map((order) => (
                <TableRow key={order.id}>
                  <TableCell>
                    <Link
                      href={`/orders/${order.id}/view`}
                      className="font-medium hover:underline"
                    >
                      {order.orderNumber}
                    </Link>
                  </TableCell>
                  <TableCell>{order.shippingFirstName ?? '—'}</TableCell>
                  <TableCell>
                    <Badge variant="outline">
                      {orderStatusLabels[order.status] ?? order.status}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">
                      {paymentStatusLabels[order.paymentStatus] ?? order.paymentStatus}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    {formatCurrency(order.total, order.currency)}
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {formatDate(order.createdAt, {
                      month: 'short',
                      day: 'numeric',
                      hour: '2-digit',
                      minute: '2-digit',
                    })}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
}
