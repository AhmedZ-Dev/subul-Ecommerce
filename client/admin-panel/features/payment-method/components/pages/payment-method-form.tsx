'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
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
import { Textarea } from '@/components/ui/textarea';
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
import { Loader2, Save, X } from 'lucide-react';
import { messages } from '@/lib/messages.ar';
import {
  createPaymentMethodSchema,
  type CreatePaymentMethodInput,
} from '../../schemas/payment-method.schema';
import { useCreatePaymentMethod, useUpdatePaymentMethod } from '../../hooks/usePaymentMethodMutations';
import { PAYMENT_METHOD_STATUS_TONES } from '../../constants';
import type { PaymentMethodDto } from '../../types';

const m = messages.paymentMethod.form;
const typeLabels = messages.paymentMethod.type;

function getDefaultValues(initialData?: PaymentMethodDto | null): CreatePaymentMethodInput {
  if (initialData) {
    return {
      name: initialData.name,
      labelEn: initialData.labelEn ?? '',
      labelAr: initialData.labelAr ?? '',
      type: initialData.type ?? '',
      gateway: initialData.gateway ?? '',
      gatewayConfig: initialData.gatewayConfig ?? '',
      iconUrl: initialData.iconUrl ?? '',
      instructionsEn: initialData.instructionsEn ?? '',
      instructionsAr: initialData.instructionsAr ?? '',
      status: initialData.status,
      sortOrder: initialData.sortOrder,
    };
  }

  return {
    name: '',
    labelEn: '',
    labelAr: '',
    type: '',
    gateway: '',
    gatewayConfig: '',
    iconUrl: '',
    instructionsEn: '',
    instructionsAr: '',
    status: 'active',
    sortOrder: 0,
  };
}

interface PaymentMethodFormProps {
  initialData?: PaymentMethodDto | null;
}

export function PaymentMethodForm({ initialData }: PaymentMethodFormProps) {
  const router = useRouter();
  const { mutateAsync: createPaymentMethodAsync, isPending: isCreating } = useCreatePaymentMethod();
  const { mutateAsync: updatePaymentMethodAsync, isPending: isUpdating } = useUpdatePaymentMethod();
  const isPending = isCreating || isUpdating;
  const isEditMode = !!initialData;
  const [leaveOpen, setLeaveOpen] = useState(false);

  const form = useForm<CreatePaymentMethodInput>({
    resolver: zodResolver(createPaymentMethodSchema) as Resolver<CreatePaymentMethodInput>,
    defaultValues: getDefaultValues(initialData),
  });

  const { isDirty } = form.formState;

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

  function buildPayload(values: CreatePaymentMethodInput) {
    return {
      name: values.name.trim().toLowerCase(),
      labelEn: values.labelEn || undefined,
      labelAr: values.labelAr || undefined,
      type: values.type || undefined,
      gateway: values.gateway || undefined,
      gatewayConfig: values.gatewayConfig || undefined,
      iconUrl: values.iconUrl || undefined,
      instructionsEn: values.instructionsEn || undefined,
      instructionsAr: values.instructionsAr || undefined,
      status: values.status,
      sortOrder: values.sortOrder,
    };
  }

  async function onSubmit(values: CreatePaymentMethodInput) {
    const payload = buildPayload(values);

    try {
      if (isEditMode && initialData) {
        await updatePaymentMethodAsync({ id: initialData.id, payload });
      } else {
        await createPaymentMethodAsync(payload);
      }
      router.push('/payment-methods');
    } catch {
      // toast handled in mutation hooks
    }
  }

  function handleCancel() {
    if (isDirty) {
      setLeaveOpen(true);
      return;
    }
    router.push('/payment-methods');
  }

  return (
    <>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.identity}</CardTitle>
                </CardHeader>
                <CardContent className="grid gap-4">
                  <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.name} *</FormLabel>
                        <FormControl>
                          <Input
                            id="payment-method-name"
                            placeholder={m.namePlaceholder}
                            dir="ltr"
                            {...field}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormDescription>{m.nameHelp}</FormDescription>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <div className="grid gap-4 md:grid-cols-2">
                    <FormField
                      control={form.control}
                      name="labelEn"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>{m.labelEn}</FormLabel>
                          <FormControl>
                            <Input
                              id="payment-method-label-en"
                              placeholder={m.labelEnPlaceholder}
                              dir="ltr"
                              {...field}
                              value={field.value ?? ''}
                              disabled={isPending}
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="labelAr"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>{m.labelAr}</FormLabel>
                          <FormControl>
                            <Input
                              id="payment-method-label-ar"
                              placeholder={m.labelArPlaceholder}
                              dir="rtl"
                              {...field}
                              value={field.value ?? ''}
                              disabled={isPending}
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </CardContent>
              </Card>

              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.sections.gateway}</CardTitle>
                </CardHeader>
                <CardContent className="grid gap-4">
                  <FormField
                    control={form.control}
                    name="type"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.type}</FormLabel>
                        <Select
                          onValueChange={(value) =>
                            field.onChange(value === 'none' ? '' : value)
                          }
                          value={field.value || 'none'}
                          disabled={isPending}
                        >
                          <FormControl>
                            <SelectTrigger id="payment-method-type">
                              <SelectValue placeholder={m.typePlaceholder} />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value="none">{m.typeNone}</SelectItem>
                            <SelectItem value="offline">{typeLabels.offline}</SelectItem>
                            <SelectItem value="online">{typeLabels.online}</SelectItem>
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="gateway"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.gateway}</FormLabel>
                        <FormControl>
                          <Input
                            id="payment-method-gateway"
                            placeholder={m.gatewayPlaceholder}
                            dir="ltr"
                            {...field}
                            value={field.value ?? ''}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="gatewayConfig"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.gatewayConfig}</FormLabel>
                        <FormControl>
                          <Textarea
                            id="payment-method-gateway-config"
                            placeholder={m.gatewayConfigPlaceholder}
                            dir="ltr"
                            className="min-h-[120px] resize-none font-mono text-xs"
                            {...field}
                            value={field.value ?? ''}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormDescription>{m.gatewayConfigHelp}</FormDescription>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="iconUrl"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.iconUrl}</FormLabel>
                        <FormControl>
                          <Input
                            id="payment-method-icon-url"
                            placeholder={m.iconUrlPlaceholder}
                            dir="ltr"
                            {...field}
                            value={field.value ?? ''}
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
                  <CardTitle className="text-base">{m.sections.instructions}</CardTitle>
                </CardHeader>
                <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <FormField
                    control={form.control}
                    name="instructionsEn"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.instructionsEn}</FormLabel>
                        <FormControl>
                          <Textarea
                            id="payment-method-instructions-en"
                            placeholder={m.instructionsEnPlaceholder}
                            dir="ltr"
                            className="min-h-[120px] resize-none"
                            {...field}
                            value={field.value ?? ''}
                            disabled={isPending}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="instructionsAr"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.instructionsAr}</FormLabel>
                        <FormControl>
                          <Textarea
                            id="payment-method-instructions-ar"
                            placeholder={m.instructionsArPlaceholder}
                            dir="rtl"
                            className="min-h-[120px] resize-none"
                            {...field}
                            value={field.value ?? ''}
                            disabled={isPending}
                          />
                        </FormControl>
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
                            tone={PAYMENT_METHOD_STATUS_TONES[field.value]}
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
                            <SelectTrigger id="payment-method-status">
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

                  <FormField
                    control={form.control}
                    name="sortOrder"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{m.sortOrder}</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            id="payment-method-sort-order"
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
            </div>
          </div>

          <FormActionsBar>
            <Button
              id="payment-method-submit-btn"
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
              id="payment-method-cancel-btn"
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
            <AlertDialogAction onClick={() => router.push('/payment-methods')}>
              {m.unsavedLeave}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
