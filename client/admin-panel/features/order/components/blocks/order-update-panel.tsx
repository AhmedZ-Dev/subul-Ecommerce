'use client';

import { useEffect } from 'react';
import { useForm, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2, Save } from 'lucide-react';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { messages } from '@/lib/messages.ar';
import {
  ORDER_FULFILLMENT_STATUSES,
  ORDER_PAYMENT_STATUSES,
  ORDER_STATUSES,
  type OrderDto,
} from '../../types';
import { updateOrderSchema, type UpdateOrderInput } from '../../schemas/order.schema';
import { useUpdateOrder } from '../../hooks/useOrderMutations';

const f = messages.order.form;
const statusLabels = messages.order.status;
const paymentLabels = messages.order.paymentStatus;
const fulfillmentLabels = messages.order.fulfillmentStatus;

function getDefaultValues(order: OrderDto): UpdateOrderInput {
  return {
    status: order.status,
    paymentStatus: order.paymentStatus,
    fulfillmentStatus: order.fulfillmentStatus,
    trackingNumber: order.trackingNumber ?? '',
    notes: order.notes ?? '',
    cancelReason: order.cancelReason ?? '',
  };
}

interface OrderUpdatePanelProps {
  order: OrderDto;
}

export function OrderUpdatePanel({ order }: OrderUpdatePanelProps) {
  const { mutate: updateOrderMutation, isPending } = useUpdateOrder();

  const form = useForm<UpdateOrderInput>({
    resolver: zodResolver(updateOrderSchema) as Resolver<UpdateOrderInput>,
    defaultValues: getDefaultValues(order),
  });

  const watchedStatus = form.watch('status');

  useEffect(() => {
    form.reset(getDefaultValues(order));
  }, [order, form]);

  function onSubmit(values: UpdateOrderInput) {
    updateOrderMutation({
      id: order.id,
      payload: {
        status: values.status,
        paymentStatus: values.paymentStatus,
        fulfillmentStatus: values.fulfillmentStatus,
        trackingNumber: values.trackingNumber || null,
        notes: values.notes || null,
        cancelReason: values.cancelReason || null,
      },
    });
  }

  return (
    <Card className="shadow-xs">
      <CardHeader>
        <CardTitle className="text-base">{f.sectionTitle}</CardTitle>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="status"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{f.status}</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger id="order-status-select">
                        <SelectValue placeholder={f.statusPlaceholder} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {ORDER_STATUSES.map((s) => (
                        <SelectItem key={s} value={s}>
                          {statusLabels[s]}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="paymentStatus"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{f.paymentStatus}</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger id="order-payment-status-select">
                        <SelectValue placeholder={f.paymentStatusPlaceholder} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {ORDER_PAYMENT_STATUSES.map((s) => (
                        <SelectItem key={s} value={s}>
                          {paymentLabels[s]}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="fulfillmentStatus"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{f.fulfillmentStatus}</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger id="order-fulfillment-status-select">
                        <SelectValue placeholder={f.fulfillmentStatusPlaceholder} />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {ORDER_FULFILLMENT_STATUSES.map((s) => (
                        <SelectItem key={s} value={s}>
                          {fulfillmentLabels[s]}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="trackingNumber"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{f.trackingNumber}</FormLabel>
                  <FormControl>
                    <Input
                      {...field}
                      dir="ltr"
                      placeholder={f.trackingNumberPlaceholder}
                      id="order-tracking-number"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="notes"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{f.notes}</FormLabel>
                  <FormControl>
                    <Textarea
                      {...field}
                      placeholder={f.notesPlaceholder}
                      rows={3}
                      id="order-notes"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {watchedStatus === 'cancelled' && (
              <FormField
                control={form.control}
                name="cancelReason"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{f.cancelReason}</FormLabel>
                    <FormControl>
                      <Textarea
                        {...field}
                        placeholder={f.cancelReasonPlaceholder}
                        rows={2}
                        id="order-cancel-reason"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            <Button type="submit" disabled={isPending} className="w-full" id="order-update-btn">
              {isPending ? (
                <Loader2 className="size-4 animate-spin" />
              ) : (
                <Save className="size-4" />
              )}
              {isPending ? f.saving : f.update}
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
