'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm, useFieldArray, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { toast } from 'sonner';
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { FormActionsBar } from '@/components/layout/form-actions-bar';
import { StatusIndicator } from '@/components/ui/status-indicator';
import { Loader2, Plus, Save, Trash2, X } from 'lucide-react';
import { messages } from '@/lib/messages.ar';
import {
  createShippingZoneSchema,
  type CreateShippingZoneInput,
} from '../../schemas/shipping-zone.schema';
import {
  useCreateShippingZone,
  useUpdateShippingZone,
} from '../../hooks/useShippingZoneMutations';
import type { ShippingZoneDto } from '../../types';
import {
  IRAQI_GOVERNORATES,
  SHIPPING_RATE_TYPE_LABELS,
  SHIPPING_ZONE_STATUS_TONES,
} from '../../constants';
import type { ShippingRatePayload } from '../../api/shipping-zone.api';

const m = messages.shippingZone.form;
const r = messages.shippingZone.rates;

function getDefaultRate(): CreateShippingZoneInput['shippingRates'][number] {
  return {
    nameEn: '',
    nameAr: '',
    rateType: 'flat',
    price: 0,
    minOrderValue: null,
    maxOrderValue: null,
    freeShippingThreshold: null,
    estimatedDaysMin: null,
    estimatedDaysMax: null,
    isActive: true,
  };
}

function getDefaultValues(initialData?: ShippingZoneDto | null): CreateShippingZoneInput {
  if (initialData) {
    return {
      nameEn: initialData.nameEn,
      nameAr: initialData.nameAr ?? '',
      governorates: initialData.governorates,
      status: initialData.status,
      shippingRates: initialData.shippingRates.map((rate) => ({
        id: rate.id,
        nameEn: rate.nameEn ?? '',
        nameAr: rate.nameAr ?? '',
        rateType: rate.rateType,
        price: rate.price,
        minOrderValue: rate.minOrderValue,
        maxOrderValue: rate.maxOrderValue,
        freeShippingThreshold: rate.freeShippingThreshold,
        estimatedDaysMin: rate.estimatedDaysMin,
        estimatedDaysMax: rate.estimatedDaysMax,
        isActive: rate.isActive,
      })),
    };
  }

  return {
    nameEn: '',
    nameAr: '',
    governorates: [],
    status: 'active',
    shippingRates: [],
  };
}

function mapRateToPayload(
  rate: CreateShippingZoneInput['shippingRates'][number],
): ShippingRatePayload {
  return {
    ...(rate.id !== undefined && rate.id > 0 ? { id: rate.id } : {}),
    nameEn: rate.nameEn || undefined,
    nameAr: rate.nameAr || undefined,
    rateType: rate.rateType,
    price: rate.price,
    minOrderValue: rate.minOrderValue ?? null,
    maxOrderValue: rate.maxOrderValue ?? null,
    freeShippingThreshold: rate.freeShippingThreshold ?? null,
    estimatedDaysMin: rate.estimatedDaysMin ?? null,
    estimatedDaysMax: rate.estimatedDaysMax ?? null,
    isActive: rate.isActive,
  };
}

interface ShippingZoneFormProps {
  initialData?: ShippingZoneDto | null;
}

export function ShippingZoneForm({ initialData }: ShippingZoneFormProps) {
  const router = useRouter();
  const { mutateAsync: createZoneAsync, isPending: isCreating } = useCreateShippingZone();
  const { mutateAsync: updateZoneAsync, isPending: isUpdating } = useUpdateShippingZone();
  const isPending = isCreating || isUpdating;
  const isEditMode = !!initialData;
  const [leaveOpen, setLeaveOpen] = useState(false);

  const form = useForm<CreateShippingZoneInput>({
    resolver: zodResolver(createShippingZoneSchema) as Resolver<CreateShippingZoneInput>,
    defaultValues: getDefaultValues(initialData),
  });

  const { isDirty } = form.formState;

  const { fields, append, remove } = useFieldArray({
    name: 'shippingRates',
    control: form.control,
  });

  useEffect(() => {
    if (initialData) {
      form.reset(getDefaultValues(initialData));
    }
  }, [initialData, form.reset]);

  useEffect(() => {
    if (!isDirty) return;
    const onBeforeUnload = (e: BeforeUnloadEvent) => {
      e.preventDefault();
    };
    window.addEventListener('beforeunload', onBeforeUnload);
    return () => window.removeEventListener('beforeunload', onBeforeUnload);
  }, [isDirty]);

  function buildCreatePayload(values: CreateShippingZoneInput) {
    return {
      nameEn: values.nameEn,
      nameAr: values.nameAr || undefined,
      governorates: values.governorates ?? [],
      isActive: values.status === 'active',
      shippingRates: (values.shippingRates ?? []).map(mapRateToPayload),
    };
  }

  function buildUpdatePayload(values: CreateShippingZoneInput) {
    return {
      nameEn: values.nameEn,
      nameAr: values.nameAr || undefined,
      governorates: values.governorates ?? [],
      isActive: values.status === 'active',
      shippingRates: (values.shippingRates ?? []).map(mapRateToPayload),
    };
  }

  async function onSubmit(values: CreateShippingZoneInput) {
    let savedZoneId: number | null = null;

    try {
      if (isEditMode && initialData) {
        await updateZoneAsync({ id: initialData.id, payload: buildUpdatePayload(values) });
        savedZoneId = initialData.id;
      } else {
        const created = await createZoneAsync(buildCreatePayload(values));
        savedZoneId = created.id;
      }

      toast.success(
        isEditMode ? messages.shippingZone.updateSuccess : messages.shippingZone.createSuccess,
      );
      router.push('/shipping-zones');
    } catch {
      if (!isEditMode && savedZoneId !== null) {
        router.replace(`/shipping-zones/${savedZoneId}/edit`);
      }
    }
  }

  function handleCancel() {
    if (isDirty) {
      setLeaveOpen(true);
      return;
    }
    router.push('/shipping-zones');
  }

  function toggleGovernorate(governorate: string, checked: boolean) {
    const current = form.getValues('governorates') ?? [];
    if (checked) {
      form.setValue('governorates', [...current, governorate], { shouldDirty: true });
    } else {
      form.setValue(
        'governorates',
        current.filter((g) => g !== governorate),
        { shouldDirty: true },
      );
    }
  }

  return (
    <>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid gap-6 lg:grid-cols-[1fr_380px]">
            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.names}</CardTitle>
                </CardHeader>
                <CardContent className="grid gap-4">
                  <FormField
                    control={form.control}
                    name="nameEn"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.nameEn} *</FormLabel>
                        <FormControl>
                          <Input
                            id="shipping-zone-name-en"
                            placeholder={m.nameEnPlaceholder}
                            dir="ltr"
                            {...field}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="nameAr"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.nameAr}</FormLabel>
                        <FormControl>
                          <Input
                            id="shipping-zone-name-ar"
                            placeholder={m.nameArPlaceholder}
                            dir="rtl"
                            {...field}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>

              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.governorates}</CardTitle>
                </CardHeader>
                <CardContent>
                  <FormField
                    control={form.control}
                    name="governorates"
                    render={({ field }) => (
                      <FormItem>
                        <FormDescription className="mb-3">{m.governoratesHelp}</FormDescription>
                        <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
                          {IRAQI_GOVERNORATES.map((gov) => {
                            const checked = (field.value ?? []).includes(gov.value);
                            return (
                              <FormItem
                                key={gov.value}
                                className="flex flex-row items-center gap-2 space-y-0 rounded-lg border border-border/50 px-3 py-2"
                              >
                                <FormControl>
                                  <Checkbox
                                    checked={checked}
                                    onCheckedChange={(value) =>
                                      toggleGovernorate(gov.value, value === true)
                                    }
                                    disabled={isPending}
                                  />
                                </FormControl>
                                <FormLabel className="cursor-pointer font-normal">
                                  {gov.labelAr}
                                  <span className="text-muted-foreground ms-1 text-xs" dir="ltr">
                                    ({gov.value})
                                  </span>
                                </FormLabel>
                              </FormItem>
                            );
                          })}
                        </div>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>
            </div>

            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.settings}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <FormField
                    control={form.control}
                    name="status"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.status}</FormLabel>
                        <div className="mb-2">
                          <StatusIndicator
                            variant="dot"
                            tone={SHIPPING_ZONE_STATUS_TONES[field.value]}
                            label={
                              field.value === 'active' ? m.statusActive : m.statusInactive
                            }
                          />
                        </div>
                        <Select
                          onValueChange={field.onChange}
                          value={field.value}
                          disabled={isPending}
                        >
                          <FormControl>
                            <SelectTrigger id="shipping-zone-status">
                              <SelectValue />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value="active">{m.statusActive}</SelectItem>
                            <SelectItem value="inactive">{m.statusInactive}</SelectItem>
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>

              <Card className="shadow-xs">
                <CardHeader className="flex flex-row items-center justify-between space-y-0">
                  <CardTitle className="text-base">{m.sections.rates}</CardTitle>
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={() => append(getDefaultRate())}
                    disabled={isPending}
                  >
                    <Plus className="size-4" />
                    {r.addRate}
                  </Button>
                </CardHeader>
                <CardContent className="space-y-4">
                  {fields.length === 0 && (
                    <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
                      {r.noRates}
                    </div>
                  )}
                  {fields.map((field, index) => (
                    <div
                      key={field.id}
                      className="relative flex flex-col gap-4 rounded-lg border border-border/50 bg-muted/20 p-4"
                    >
                      <div className="flex items-start justify-between gap-2">
                        <p className="text-sm font-medium">
                          {r.rateNumber(index + 1)}
                        </p>
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon-sm"
                          onClick={() => remove(index)}
                          className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                          disabled={isPending}
                          aria-label={messages.common.delete}
                        >
                          <Trash2 className="size-4" />
                        </Button>
                      </div>

                      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.nameEn`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.nameEn}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  dir="ltr"
                                  {...rateField}
                                  value={rateField.value ?? ''}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.nameAr`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.nameAr}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  dir="rtl"
                                  {...rateField}
                                  value={rateField.value ?? ''}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.rateType`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.rateType}</FormLabel>
                              <Select
                                disabled={isPending}
                                onValueChange={rateField.onChange}
                                value={rateField.value}
                              >
                                <FormControl>
                                  <SelectTrigger>
                                    <SelectValue />
                                  </SelectTrigger>
                                </FormControl>
                                <SelectContent>
                                  {(['flat', 'weight_based', 'price_based'] as const).map(
                                    (type) => (
                                      <SelectItem key={type} value={type}>
                                        {SHIPPING_RATE_TYPE_LABELS[type]}
                                      </SelectItem>
                                    ),
                                  )}
                                </SelectContent>
                              </Select>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.price`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.price} *</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  type="number"
                                  min={0}
                                  step="0.01"
                                  dir="ltr"
                                  {...rateField}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.minOrderValue`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.minOrderValue}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  type="number"
                                  min={0}
                                  step="0.01"
                                  dir="ltr"
                                  {...rateField}
                                  value={rateField.value ?? ''}
                                  onChange={(e) =>
                                    rateField.onChange(
                                      e.target.value === '' ? null : Number(e.target.value),
                                    )
                                  }
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.maxOrderValue`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.maxOrderValue}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  type="number"
                                  min={0}
                                  step="0.01"
                                  dir="ltr"
                                  {...rateField}
                                  value={rateField.value ?? ''}
                                  onChange={(e) =>
                                    rateField.onChange(
                                      e.target.value === '' ? null : Number(e.target.value),
                                    )
                                  }
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.freeShippingThreshold`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.freeShippingThreshold}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  type="number"
                                  min={0}
                                  step="0.01"
                                  dir="ltr"
                                  {...rateField}
                                  value={rateField.value ?? ''}
                                  onChange={(e) =>
                                    rateField.onChange(
                                      e.target.value === '' ? null : Number(e.target.value),
                                    )
                                  }
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.estimatedDaysMin`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.estimatedDaysMin}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  type="number"
                                  min={0}
                                  dir="ltr"
                                  {...rateField}
                                  value={rateField.value ?? ''}
                                  onChange={(e) =>
                                    rateField.onChange(
                                      e.target.value === '' ? null : Number(e.target.value),
                                    )
                                  }
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.estimatedDaysMax`}
                          render={({ field: rateField }) => (
                            <FormItem>
                              <FormLabel>{r.estimatedDaysMax}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  type="number"
                                  min={0}
                                  dir="ltr"
                                  {...rateField}
                                  value={rateField.value ?? ''}
                                  onChange={(e) =>
                                    rateField.onChange(
                                      e.target.value === '' ? null : Number(e.target.value),
                                    )
                                  }
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`shippingRates.${index}.isActive`}
                          render={({ field: rateField }) => (
                            <FormItem className="flex flex-row items-center justify-between rounded-lg border p-3 sm:col-span-2">
                              <FormLabel className="text-sm">{r.isActive}</FormLabel>
                              <FormControl>
                                <Checkbox
                                  checked={rateField.value}
                                  onCheckedChange={rateField.onChange}
                                  disabled={isPending}
                                />
                              </FormControl>
                            </FormItem>
                          )}
                        />
                      </div>
                    </div>
                  ))}
                </CardContent>
              </Card>
            </div>
          </div>

          <FormActionsBar>
            <Button
              id="shipping-zone-submit-btn"
              type="submit"
              disabled={isPending}
              className="min-w-[130px]"
            >
              {isPending ? (
                <>
                  <Loader2 className="size-4 animate-spin" />
                  {m.saving}
                </>
              ) : (
                <>
                  <Save className="size-4" />
                  {isEditMode ? m.update : m.create}
                </>
              )}
            </Button>
            <Button
              id="shipping-zone-cancel-btn"
              type="button"
              variant="outline"
              disabled={isPending}
              onClick={handleCancel}
            >
              <X className="size-4" />
              {m.cancel}
            </Button>
          </FormActionsBar>
        </form>
      </Form>

      <AlertDialog open={leaveOpen} onOpenChange={setLeaveOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.unsavedTitle}</AlertDialogTitle>
            <AlertDialogDescription>{m.unsavedDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{m.unsavedStay}</AlertDialogCancel>
            <AlertDialogAction onClick={() => router.push('/shipping-zones')}>
              {m.unsavedLeave}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
