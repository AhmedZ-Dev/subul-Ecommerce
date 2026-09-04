"use client"

import { useMemo } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Banknote } from "lucide-react"
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
import { Textarea } from "@/components/ui/textarea"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { useActiveShippingZones } from "@/features/shipping-zone"
import { IRAQ_GOVERNORATES } from "../../constants"
import { checkoutSchema, type CheckoutFormValues } from "../../schemas/checkout.schema"
import { messages } from "@/lib/messages.ar"

interface CheckoutFormProps {
  onSubmit: (values: CheckoutFormValues) => void
  isSubmitting?: boolean
}

export function CheckoutForm({ onSubmit, isSubmitting }: CheckoutFormProps) {
  const { data: zones = [] } = useActiveShippingZones()

  // One governorate picker drives the shipping zone too — the customer should
  // not have to state the same location twice.
  const governorateOptions = useMemo(() => {
    const zoneByGovernorate = new Map<string, number>()
    for (const zone of zones) {
      for (const governorate of zone.governorates ?? []) {
        zoneByGovernorate.set(governorate.trim().toLowerCase(), zone.id)
      }
    }
    return IRAQ_GOVERNORATES.map((item) => ({
      ...item,
      zoneId: zoneByGovernorate.get(item.en.toLowerCase()),
    }))
  }, [zones])

  const form = useForm<CheckoutFormValues>({
    resolver: zodResolver(checkoutSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      phone: "",
      address1: "",
      address2: "",
      city: "",
      governorate: "",
      paymentMethod: "cod",
      customerNotes: "",
    },
  })

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-6">
        <div>
          <h2 className="mb-4 text-lg font-semibold">{messages.checkout.shippingInfo}</h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <FormField
              control={form.control}
              name="firstName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{messages.checkout.fields.firstName}</FormLabel>
                  <FormControl>
                    <Input {...field} autoComplete="given-name" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="lastName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{messages.checkout.fields.lastName}</FormLabel>
                  <FormControl>
                    <Input {...field} autoComplete="family-name" />
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
                  <FormLabel>{messages.checkout.fields.phone}</FormLabel>
                  <FormControl>
                    <Input {...field} dir="ltr" autoComplete="tel" inputMode="tel" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="governorate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{messages.checkout.fields.governorate}</FormLabel>
                  <Select
                    value={field.value || undefined}
                    onValueChange={(value) => {
                      field.onChange(value)
                      form.setValue(
                        "shippingZoneId",
                        governorateOptions.find((item) => item.ar === value)?.zoneId,
                      )
                    }}
                  >
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue
                          placeholder={messages.checkout.fields.governoratePlaceholder}
                        />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {governorateOptions.map((item) => (
                        <SelectItem key={item.en} value={item.ar}>
                          {item.ar}
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
              name="city"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{messages.checkout.fields.city}</FormLabel>
                  <FormControl>
                    <Input
                      {...field}
                      autoComplete="address-level2"
                      placeholder={messages.checkout.fields.cityPlaceholder}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="address1"
              render={({ field }) => (
                <FormItem className="sm:col-span-2">
                  <FormLabel>{messages.checkout.fields.address1}</FormLabel>
                  <FormControl>
                    <Input {...field} autoComplete="street-address" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="address2"
              render={({ field }) => (
                <FormItem className="sm:col-span-2">
                  <FormLabel>{messages.checkout.fields.address2}</FormLabel>
                  <FormControl>
                    <Input {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        </div>

        <div className="bg-muted/40 flex items-start gap-3 rounded-2xl border border-foreground/8 p-4">
          <span className="bg-primary/10 text-primary flex size-10 shrink-0 items-center justify-center rounded-xl">
            <Banknote className="size-5" aria-hidden />
          </span>
          <div className="min-w-0">
            <p className="font-semibold">{messages.checkout.paymentCod}</p>
            <p className="text-muted-foreground mt-0.5 text-sm leading-relaxed">
              {messages.checkout.paymentCodDescription}
            </p>
          </div>
        </div>

        <FormField
          control={form.control}
          name="customerNotes"
          render={({ field }) => (
            <FormItem>
              <FormLabel>{messages.checkout.customerNotes}</FormLabel>
              <FormControl>
                <Textarea
                  {...field}
                  placeholder={messages.checkout.customerNotesPlaceholder}
                  rows={3}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <Button
          type="submit"
          size="lg"
          className="h-12 w-full rounded-full text-base font-semibold sm:w-auto sm:min-w-56"
          disabled={isSubmitting}
        >
          {isSubmitting ? messages.common.loading : messages.checkout.placeOrder}
        </Button>
      </form>
    </Form>
  )
}
