'use client';

import Link from 'next/link';
import { ArrowRight, ClipboardList } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardContent,
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
import { Separator } from '@/components/ui/separator';
import { formatCurrency, formatDate, messages } from '@/lib/messages.ar';
import { cn } from '@/lib/utils';
import type { OrderDto } from '../../types';
import { useOrder } from '../../hooks/useOrder';
import { OrderStatusBadge } from '../blocks/order-status-badge';
import { OrderPaymentStatusBadge } from '../blocks/order-payment-status-badge';
import { OrderFulfillmentStatusBadge } from '../blocks/order-fulfillment-status-badge';
import { OrderUpdatePanel } from '../blocks/order-update-panel';

function DetailField({
  label,
  value,
  dir,
  className,
}: {
  label: string;
  value: React.ReactNode;
  dir?: 'rtl' | 'ltr' | 'auto';
  className?: string;
}) {
  return (
    <div
      className={cn(
        'space-y-1.5 rounded-lg border border-border/50 bg-muted/20 px-3 py-2.5',
        className,
      )}
    >
      <dt className="text-muted-foreground text-xs font-medium">{label}</dt>
      <dd className="text-sm leading-relaxed" dir={dir}>
        {value}
      </dd>
    </div>
  );
}

function PricingRow({
  label,
  value,
  bold,
}: {
  label: string;
  value: string;
  bold?: boolean;
}) {
  return (
    <div className="flex items-center justify-between gap-4 text-sm">
      <span className="text-muted-foreground">{label}</span>
      <span dir="ltr" className={cn('tabular-nums', bold && 'text-base font-semibold')}>
        {value}
      </span>
    </div>
  );
}

interface OrderViewProps {
  order: OrderDto;
}

export function OrderView({ order }: OrderViewProps) {
  const { data: liveOrder } = useOrder(order.id, { initialData: order });
  const o = liveOrder ?? order;

  const m = messages.order.view;
  const customerName = [o.shippingFirstName, o.shippingLastName].filter(Boolean).join(' ');
  const addressLines = [
    o.shippingAddress1,
    o.shippingAddress2,
    [o.shippingCity, o.shippingGovernorate].filter(Boolean).join('، '),
    o.shippingCountry,
  ].filter(Boolean);

  return (
    <>
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div className="space-y-1">
          <Button variant="ghost" size="sm" className="-ms-2 mb-1" asChild>
            <Link href="/orders">
              <ArrowRight className="size-4" />
              {m.backToList}
            </Link>
          </Button>
          <div className="flex items-center gap-3">
            <div className="bg-muted flex size-10 shrink-0 items-center justify-center rounded-lg">
              <ClipboardList className="text-primary size-5" />
            </div>
            <div>
              <h1 className="text-xl font-semibold tracking-tight" dir="ltr">
                {o.orderNumber}
              </h1>
              <p className="text-muted-foreground text-sm">
                {customerName || m.noCustomer}
              </p>
            </div>
          </div>
          <div className="flex flex-wrap items-center gap-2 pt-1">
            <OrderStatusBadge status={o.status} />
            <OrderPaymentStatusBadge status={o.paymentStatus} />
            <OrderFulfillmentStatusBadge status={o.fulfillmentStatus} />
          </div>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
        <div className="space-y-6">
          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{m.items}</CardTitle>
            </CardHeader>
            <CardContent>
              {o.items.length > 0 ? (
                <div className="overflow-x-auto rounded-lg border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>{m.itemProduct}</TableHead>
                        <TableHead>{m.itemSku}</TableHead>
                        <TableHead>{m.itemQuantity}</TableHead>
                        <TableHead>{m.itemUnitPrice}</TableHead>
                        <TableHead>{m.itemTotal}</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {o.items.map((item) => (
                        <TableRow key={item.id}>
                          <TableCell>{item.productName}</TableCell>
                          <TableCell dir="ltr">{item.sku ?? m.empty}</TableCell>
                          <TableCell dir="ltr">{item.quantity}</TableCell>
                          <TableCell dir="ltr">
                            {formatCurrency(item.unitPrice, o.currency)}
                          </TableCell>
                          <TableCell dir="ltr">
                            {formatCurrency(item.totalPrice, o.currency)}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              ) : (
                <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
                  {m.noItems}
                </div>
              )}
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{m.shippingAddress}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                <DetailField label={m.customer} value={customerName || m.empty} />
                <DetailField
                  label={m.phone}
                  value={o.shippingPhone ?? m.empty}
                  dir="ltr"
                />
                <DetailField
                  label={m.address}
                  value={
                    addressLines.length > 0 ? (
                      <span className="block space-y-0.5">
                        {addressLines.map((line, i) => (
                          <span key={i} className="block">
                            {line}
                          </span>
                        ))}
                      </span>
                    ) : (
                      m.empty
                    )
                  }
                  className="sm:col-span-2"
                />
                {o.trackingNumber && (
                  <DetailField
                    label={m.trackingNumber}
                    value={o.trackingNumber}
                    dir="ltr"
                  />
                )}
                {o.customerNotes && (
                  <DetailField
                    label={m.customerNotes}
                    value={o.customerNotes}
                    className="sm:col-span-2"
                  />
                )}
              </dl>
            </CardContent>
          </Card>
        </div>

        <div className="space-y-6">
          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{m.pricing}</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <PricingRow
                label={m.subtotal}
                value={formatCurrency(o.subtotal, o.currency)}
              />
              <PricingRow
                label={m.discount}
                value={formatCurrency(o.discountAmount, o.currency)}
              />
              {o.couponCode && (
                <PricingRow label={m.couponCode} value={o.couponCode} />
              )}
              <PricingRow
                label={m.shipping}
                value={formatCurrency(o.shippingAmount, o.currency)}
              />
              <PricingRow label={m.tax} value={formatCurrency(o.taxAmount, o.currency)} />
              <Separator />
              <PricingRow
                label={m.total}
                value={formatCurrency(o.total, o.currency)}
                bold
              />
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{m.paymentInfo}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="space-y-3">
                <DetailField
                  label={m.paymentMethod}
                  value={o.paymentMethod ?? m.empty}
                  dir="ltr"
                />
                <DetailField
                  label={m.paymentStatus}
                  value={<OrderPaymentStatusBadge status={o.paymentStatus} />}
                />
                <DetailField label={m.currency} value={o.currency} dir="ltr" />
              </dl>
            </CardContent>
          </Card>

          <Card className="shadow-xs">
            <CardHeader>
              <CardTitle className="text-base">{m.timestamps}</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="space-y-3">
                <DetailField
                  label={m.createdAt}
                  value={formatDate(o.createdAt)}
                  dir="ltr"
                />
                <DetailField
                  label={m.updatedAt}
                  value={o.updatedAt ? formatDate(o.updatedAt) : m.notUpdated}
                  dir="ltr"
                />
                {o.shippedAt && (
                  <DetailField
                    label={m.shippedAt}
                    value={formatDate(o.shippedAt)}
                    dir="ltr"
                  />
                )}
                {o.deliveredAt && (
                  <DetailField
                    label={m.deliveredAt}
                    value={formatDate(o.deliveredAt)}
                    dir="ltr"
                  />
                )}
                {o.cancelledAt && (
                  <DetailField
                    label={m.cancelledAt}
                    value={formatDate(o.cancelledAt)}
                    dir="ltr"
                  />
                )}
                {o.cancelReason && (
                  <DetailField label={m.cancelReason} value={o.cancelReason} />
                )}
              </dl>
            </CardContent>
          </Card>

          <OrderUpdatePanel order={o} />
        </div>
      </div>
    </>
  );
}
