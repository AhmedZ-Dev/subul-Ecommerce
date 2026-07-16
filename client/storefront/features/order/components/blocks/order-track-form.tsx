"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Button } from "@/components/ui/button"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Card, CardContent } from "@/components/ui/card"
import { OrderStatusTimeline } from "./order-status-timeline"
import { useTrackOrder } from "../../hooks/useOrder"
import { orderTrackSchema, type OrderTrackFormValues } from "../../schemas/order-track.schema"
import { formatCurrency, formatDate, messages } from "@/lib/messages.ar"
import type { GuestOrderTrackResult } from "../../types"

export function OrderTrackForm() {
  const trackOrder = useTrackOrder()
  const [result, setResult] = useState<GuestOrderTrackResult | null>(null)
  const [notFound, setNotFound] = useState(false)

  const form = useForm<OrderTrackFormValues>({
    resolver: zodResolver(orderTrackSchema),
    defaultValues: { orderNumber: "", phone: "" },
  })

  async function onSubmit(values: OrderTrackFormValues) {
    setNotFound(false)
    setResult(null)
    const data = await trackOrder.mutateAsync(values)
    if (!data) {
      setNotFound(true)
    } else {
      setResult(data)
    }
  }

  return (
    <div className="mx-auto flex max-w-lg flex-col gap-6">
      <div>
        <h1 className="text-2xl font-bold">{messages.order.trackTitle}</h1>
        <p className="text-muted-foreground mt-1 text-sm">{messages.order.trackDescription}</p>
      </div>

      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">
          <FormField
            control={form.control}
            name="orderNumber"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{messages.order.orderNumber}</FormLabel>
                <FormControl>
                  <Input
                    {...field}
                    placeholder={messages.order.orderNumberPlaceholder}
                    dir="ltr"
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="phone"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{messages.order.phone}</FormLabel>
                <FormControl>
                  <Input
                    {...field}
                    placeholder={messages.order.phonePlaceholder}
                    dir="ltr"
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <Button type="submit" disabled={trackOrder.isPending}>
            {trackOrder.isPending ? messages.common.loading : messages.order.track}
          </Button>
        </form>
      </Form>

      {notFound && (
        <p className="text-destructive text-center text-sm">{messages.order.notFound}</p>
      )}

      {result && (
        <Card>
          <CardContent className="flex flex-col gap-6 p-6">
            <div>
              <p className="text-muted-foreground text-sm">{messages.order.orderNumber}</p>
              <p className="font-mono text-lg font-bold" dir="ltr">
                {result.orderNumber}
              </p>
            </div>

            <OrderStatusTimeline status={result.status} />

            <div className="flex flex-col gap-2 text-sm">
              <div className="flex justify-between">
                <span className="text-muted-foreground">{messages.order.details.total}</span>
                <span className="font-semibold">
                  {formatCurrency(result.total, result.currency)}
                </span>
              </div>
              {result.shippingCity && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">{messages.order.details.shippingTo}</span>
                  <span>
                    {result.shippingCity}
                    {result.shippingGovernorate ? `, ${result.shippingGovernorate}` : ""}
                  </span>
                </div>
              )}
              {result.trackingNumber && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">
                    {messages.order.details.trackingNumber}
                  </span>
                  <span dir="ltr">{result.trackingNumber}</span>
                </div>
              )}
              <div className="flex justify-between">
                <span className="text-muted-foreground">{messages.order.details.createdAt}</span>
                <span>{formatDate(result.createdAt)}</span>
              </div>
            </div>

            {result.items.length > 0 && (
              <div>
                <p className="mb-2 font-medium">{messages.order.details.items}</p>
                <div className="flex flex-col gap-2">
                  {result.items.map((item, i) => (
                    <div key={i} className="flex justify-between text-sm">
                      <span>
                        {item.productName} × {item.quantity}
                      </span>
                      <span>{formatCurrency(item.totalPrice, result.currency)}</span>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </CardContent>
        </Card>
      )}
    </div>
  )
}
